using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Diagraph.Infrastructure.Dynamic.Extensions;
using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.ProcessManager.Contracts;
using Diagraph.Infrastructure.Tests;
using Diagraph.Infrastructure.Tests.AutoFixture;
using Diagraph.Modules.Identity.Database;
using Diagraph.Modules.Identity.UserData;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Integration.UserData.Events;
using Xunit;
// ReSharper disable CoVariantArrayConversion

namespace Diagraph.Modules.Identity.Tests.Integration;

[Collection(nameof(IdentityFixtureCollection))]
public class DataRemovalProcessManagerTests
{
    private readonly  IdentityFixture _fixture;

    public DataRemovalProcessManagerTests(IdentityFixture fixture) => _fixture = fixture;

    [Theory, CustomizedAutoData]
    public async Task Schedules_Process(string email)
    {
        // Arrange
        var pm = _fixture.Module.Service<IProcessManager<UserDataRemovalState>>();
        UserDataRemovalState state = UserDataRemovalState.Create(email);
        
        // Act
        await pm.ScheduleAsync(state);
        
        // Assert
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            Process process = await context.Processes.SingleOrDefaultAsync(p => p.ProcessId == email);
            process.Should().NotBeNull();
        });
    }
    
    [Theory, CustomizedAutoData]
    public async Task WakeUp_Initiates_Process(string email)
    {
        // Arrange
        var pm = _fixture.Module.Service<IProcessManager<UserDataRemovalState>>();
        UserDataRemovalState initialState = UserDataRemovalState.Create(email);
        
        await pm.ScheduleAsync(initialState);

        Process process = null;
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            process = await context.Processes.SingleOrDefaultAsync(p => p.ProcessId == email);
        });
        
        // Act
        await pm.WakeUpAsync(process);
        
        // Assert
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            Process updatedProcess = await context
                .Processes
                .SingleOrDefaultAsync(p => p.ProcessId == email);
            updatedProcess!.Initiated.Should().BeTrue();

            var state = updatedProcess.GetData<UserDataRemovalState>();
            state.Should().NotBeNull();
            state.Initiated.Should().BeTrue();
        });
        
        var events = await _fixture.EventStore.Events(initialState.Key());
        events.Should().NotBeNullOrEmpty();

        events.Should().ContainItemsAssignableTo<UserDataRemovalProcessCreated>();
        events.Should().ContainItemsAssignableTo<RequestedUserDataRemovalEvent>();
        events.Should().ContainItemsAssignableTo<RequestedEventDataRemovalEvent>();
        events.Should().ContainItemsAssignableTo<RequestedGlucoseDataRemovalEvent>();
        events.Should().ContainItemsAssignableTo<RequestedGlucoseDataRemovalEvent>();
    }

    [Theory, CustomizedAutoData]
    public async Task Updates_State_When_Event_Dispatched(string email)
    {
        // Arrange
        var pm = _fixture.Module.Service<IProcessManager<UserDataRemovalState>>();
        UserDataRemovalState initialState = UserDataRemovalState.Create(email);

        await pm.ScheduleAsync(initialState);
    
        Process process = null;
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context => { process = await context.Processes.SingleOrDefaultAsync(p => p.ProcessId == email); });

        await pm.WakeUpAsync(process);
        
        AutoResetEvent handle = new(false);
        
        await TestHandlerWrapper.RunAsync
        (
            _fixture.Module.Service<EventSubscriber>(), 
            pm, 
            (@event, _) =>
            {
                if (@event is EventDataRemovedEvent)
                    handle.Set();
            }
        );
    
        // Act
        var @event = new EventDataRemovedEvent(email);
        await _fixture.EventStore.DispatchEvent(initialState.Key(), @event);

        handle.WaitOne();

        // Assert
        var events = await _fixture.EventStore.Events(initialState.Key());
        events.Should().NotBeNullOrEmpty();
    
        events.Should().ContainItemsAssignableTo<EventDataRemovedEvent>();
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            Process updatedProcess = await context.Processes.SingleOrDefaultAsync(p => p.ProcessId == email);
            var     latestState    = updatedProcess.GetData<UserDataRemovalState>();
            latestState.EventDataRemoved.Should().BeTrue();
        });
    }
    
    [Theory, CustomizedAutoData]
    public async Task Dispatches_RequestAccountRemovalEvent_When_Event_And_Glucose_Data_Deleted
    (
        string email
    )
    {
        // Arrange
        var pm = _fixture.Module.Service<IProcessManager<UserDataRemovalState>>();
        UserDataRemovalState initialState = UserDataRemovalState.Create(email);

        await pm.ScheduleAsync(initialState);
    
        Process process = null;
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context => { process = await context.Processes.SingleOrDefaultAsync(p => p.ProcessId == email); });

        await pm.WakeUpAsync(process);
        
        Dictionary<Type, AutoResetEvent> handles = new()
        {
            [typeof(EventDataRemovedEvent)] = new AutoResetEvent(false),
            [typeof(GlucoseDataRemovedEvent)] = new AutoResetEvent(false)
        };
        await TestHandlerWrapper.RunAsync
        (
            _fixture.Module.Service<EventSubscriber>(), 
            pm, 
            (@event, _) =>
            {
                Type type = @event.GetType();
                if (handles.ContainsKey(type))
                    handles[type].Set();
            }
        );

        // Act
        var eventDataRemovedEvent = new EventDataRemovedEvent(email);
        await _fixture.EventStore.DispatchEvent(initialState.Key(), eventDataRemovedEvent);
    
        var glucoseDataRemovedEvent = new GlucoseDataRemovedEvent(email);
        await _fixture.EventStore.DispatchEvent(initialState.Key(), glucoseDataRemovedEvent);
    
        WaitHandle.WaitAll(handles.Values.ToArray()).Should().BeTrue();
    
        // Assert
        var events = await _fixture.EventStore.Events(initialState.Key());
        events.Should().NotBeNullOrEmpty();
    
        events.Should().ContainItemsAssignableTo<RequestedUserAccountRemovalEvent>();
        await _fixture.Module.ExecuteAsync<IdentityDbContext>(async context =>
        {
            Process updatedProcess = await context
                .Processes
                .SingleOrDefaultAsync(p => p.ProcessId == email);
            var latestState = updatedProcess.GetData<UserDataRemovalState>();

            latestState.DataRemoved.Should().BeTrue();
        });
    }
}
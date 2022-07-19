using Diagraph.Infrastructure.EventSourcing;
using Modules.Identity.Integration.UserData.Events;

namespace Diagraph.Modules.Identity.UserData;

public class UserDataRemovalState : AggregateEntity<string>
{
    public bool DataRemoved => EventDataRemoved && GlucoseDataRemoved;
    
    public bool UserRemoved { get; set; }
    
    public bool Initiated { get; set; }
    
    public bool RequestedEventDataRemoval { get; set; }
    
    public bool EventDataRemoved { get; set; }
    
    public bool RequestedGlucoseDataRemoval { get; set; }
    
    public bool GlucoseDataRemoved { get; set; }
    
    public bool RequestedAccountRemoval { get; set; }
    
    public bool AccountRemoved { get; set; }
    
    public static UserDataRemovalState Create(string userName)
    {
        UserDataRemovalState state = new();
        state.ApplyEvent(new UserDataRemovalProcessCreated(userName));
        return state;
    }

    private void Apply(UserDataRemovalProcessCreated @event)
        => Id = @event.UserName;

    public void RequestRemoval() => ApplyEvent(new RequestedUserDataRemovalEvent(Id));
    
    private void Apply(RequestedUserDataRemovalEvent _) => Initiated = true;

    public void RequestEventDataRemoval()
        => ApplyEvent(new RequestedEventDataRemovalEvent(Id));
    
    private void Apply(RequestedEventDataRemovalEvent _)
        => RequestedEventDataRemoval = true;

    public void MarkEventDataRemoved()
        => ApplyEvent(new EventDataRemovedEvent(Id));
    
    private void Apply(EventDataRemovedEvent _)
        => EventDataRemoved = true;
    
    public void RequestGlucoseDataRemoval()
        => ApplyEvent(new RequestedGlucoseDataRemovalEvent(Id));

    private void Apply(RequestedGlucoseDataRemovalEvent _) 
        => RequestedGlucoseDataRemoval = true;

    public void MarkGlucoseDataRemoved()
        => ApplyEvent(new GlucoseDataRemovedEvent(Id));

    private void Apply(GlucoseDataRemovedEvent _)
        => GlucoseDataRemoved = true;
    
    public void RequestUserAccountRemoval()
        => ApplyEvent(new RequestedUserAccountRemovalEvent(Id));

    private void Apply(RequestedUserAccountRemovalEvent _)
        => RequestedAccountRemoval = true;

    public void MarkAccountRemoved()
        => ApplyEvent(new AccountRemovedEvent(Id));
    
    private void Apply(AccountRemovedEvent _)
        => AccountRemoved = true;

    public void MarkUserDataRemoved()
        => ApplyEvent(new UserDataRemovedEvent(Id));

    private void Apply(UserDataRemovedEvent _)
        => UserRemoved = true;
    
    protected override void RegisterAppliers()
    {
        RegisterApplier<UserDataRemovalProcessCreated>(Apply);
        
        RegisterApplier<RequestedUserDataRemovalEvent>(Apply);
        RegisterApplier<UserDataRemovedEvent>(Apply); 
        
        RegisterApplier<RequestedEventDataRemovalEvent>(Apply);
        RegisterApplier<EventDataRemovedEvent>(Apply);
        
        RegisterApplier<RequestedGlucoseDataRemovalEvent>(Apply);
        RegisterApplier<GlucoseDataRemovedEvent>(Apply);

        RegisterApplier<RequestedUserAccountRemovalEvent>(Apply);
        RegisterApplier<AccountRemovedEvent>(Apply);
    } 
}
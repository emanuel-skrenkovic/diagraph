using Diagraph.Infrastructure.EventSourcing;
using Diagraph.Infrastructure.EventSourcing.Contracts;
using Diagraph.Infrastructure.Tests;

namespace Diagraph.Shared.Tests.Integration.EventStore;

public class EventStoreFixture : EventStoreModuleFixture
{
    public EventStoreFixture() : base("shared") { }
    
    #region TestClasses
    
    internal record TestCreateEvent(Guid Id): IEvent;
    internal record IntSetEvent(int Value): IEvent;
    internal record StringSetEvent(string Value): IEvent;
    internal record ObjectSetEvent(int TestInt, string TestString) : IEvent;
    
    internal class TestObject 
    {
        public int    TestInt    { get; set; }
        public string TestString { get; set; } 
    }
    
    internal class TestAggregate : AggregateEntity<Guid>
    {
        public int        TestInt    { get; private set; }
        public string     TestString { get; private set; }
        public TestObject TestObject { get; private set; }
    
        public static TestAggregate Create()
        {
            var agg = new TestAggregate();
            agg.ApplyEvent(new TestCreateEvent(Guid.NewGuid()));
            return agg;
        }
    
        private void Apply(TestCreateEvent @event) 
            => Id = @event.Id;
    
        internal void SetInt(int value) 
            => ApplyEvent(new IntSetEvent(value));
    
        private void Apply(IntSetEvent @event) 
            => TestInt = @event.Value;
    
        internal void SetString(string value) 
            => ApplyEvent(new StringSetEvent(value));
    
        private void Apply(StringSetEvent @event) 
            => TestString = @event.Value;
    
        internal void SetObject(int testInt, string testString) 
            => ApplyEvent(new ObjectSetEvent(testInt, testString));
    
        private void Apply(ObjectSetEvent @event) 
            => TestObject = new TestObject
            {
                TestInt = @event.TestInt, 
                TestString = @event.TestString
            };
    
        protected override void RegisterAppliers()
        {
            RegisterApplier<TestCreateEvent>(Apply);
            RegisterApplier<IntSetEvent>(Apply);
            RegisterApplier<StringSetEvent>(Apply);
            RegisterApplier<ObjectSetEvent>(Apply);
        }
    }
    
    #endregion
}
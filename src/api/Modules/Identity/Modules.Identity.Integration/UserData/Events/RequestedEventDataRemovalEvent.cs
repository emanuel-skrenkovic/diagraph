using Diagraph.Infrastructure.EventSourcing.Contracts;

namespace Modules.Identity.Integration.UserData.Events;

public record RequestedEventDataRemovalEvent(string UserName) : IEvent;
using Diagraph.Infrastructure.EventSourcing.Contracts;

namespace Modules.Identity.Integration.UserData.Events;

public record RequestedUserDataRemovalEvent(string UserName) : IEvent;
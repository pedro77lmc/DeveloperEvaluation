using Domain.Events;

namespace Application.Services
{
    public interface IDomainEventHandler
    {
        bool CanHandle(Type eventType);
        Task HandleAsync(IDomainEvent domainEvent);
    }
}

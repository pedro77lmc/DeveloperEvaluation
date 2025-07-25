using Domain.Events;

namespace Application.Services
{
    public class SaleCancelledEventHandler : IDomainEventHandler
    {
        private readonly ILogger<SaleCancelledEventHandler> _logger;

        public SaleCancelledEventHandler(ILogger<SaleCancelledEventHandler> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(Type eventType) => eventType == typeof(SaleCancelledEvent);

        public Task HandleAsync(IDomainEvent domainEvent)
        {
            if (domainEvent is SaleCancelledEvent saleCancelled)
            {
                _logger.LogWarning("❌ SALE CANCELLED! Sale #{SaleNumber} (ID: {SaleId})",
                    saleCancelled.SaleNumber, saleCancelled.SaleId);

                // Here you could:
                // - Restore inventory
                // - Send cancellation notifications
                // - Update customer credit
                // - Trigger refund process
            }

            return Task.CompletedTask;
        }
    }
}

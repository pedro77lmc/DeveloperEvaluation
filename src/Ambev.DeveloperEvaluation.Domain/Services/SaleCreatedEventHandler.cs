using Domain.Events;

namespace Application.Services
{
    public class SaleCreatedEventHandler : IDomainEventHandler
    {
        private readonly ILogger<SaleCreatedEventHandler> _logger;

        public SaleCreatedEventHandler(ILogger<SaleCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(Type eventType) => eventType == typeof(SaleCreatedEvent);

        public Task HandleAsync(IDomainEvent domainEvent)
        {
            if (domainEvent is SaleCreatedEvent saleCreated)
            {
                _logger.LogInformation("🎉 NEW SALE CREATED! Sale #{SaleNumber} for Customer {CustomerId} - Total: ${TotalAmount:F2}",
                    saleCreated.SaleNumber, saleCreated.CustomerId, saleCreated.TotalAmount);

                // Here you could:
                // - Send notification emails
                // - Update analytics
                // - Trigger inventory updates
                // - Publish to message broker
            }

            return Task.CompletedTask;
        }
    }
}

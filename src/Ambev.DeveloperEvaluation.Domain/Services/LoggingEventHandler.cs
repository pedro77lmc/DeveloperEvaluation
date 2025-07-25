using Domain.Events;

namespace Application.Services
{
    public class LoggingEventHandler : IDomainEventHandler
    {
        private readonly ILogger<LoggingEventHandler> _logger;

        public LoggingEventHandler(ILogger<LoggingEventHandler> logger)
        {
            _logger = logger;
        }

        public bool CanHandle(Type eventType) => typeof(IDomainEvent).IsAssignableFrom(eventType);

        public Task HandleAsync(IDomainEvent domainEvent)
        {
            var eventData = System.Text.Json.JsonSerializer.Serialize(domainEvent, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });

            _logger.LogInformation("📢 Domain Event Published: {EventType}\n{EventData}",
                domainEvent.GetType().Name, eventData);

            return Task.CompletedTask;
        }
    }
}

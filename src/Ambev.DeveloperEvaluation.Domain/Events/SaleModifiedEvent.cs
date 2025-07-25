
namespace Domain.Events
{
    public class SaleModifiedEvent : IDomainEvent
    {
        private Guid id;
        private decimal totalAmount;

        public SaleModifiedEvent(Guid id, decimal totalAmount)
        {
            this.id = id;
            this.totalAmount = totalAmount;
        }

        public Guid Id => throw new NotImplementedException();

        public DateTime OccurredOn => throw new NotImplementedException();
    }
}
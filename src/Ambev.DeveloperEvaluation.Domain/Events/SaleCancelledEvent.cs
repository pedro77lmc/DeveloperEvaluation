namespace Domain.Events
{
    public class SaleCancelledEvent : DomainEvent
    {
        public Guid SaleId { get; }
        public string SaleNumber { get; }

        public SaleCancelledEvent(Guid saleId, string saleNumber)
        {
            SaleId = saleId;
            SaleNumber = saleNumber;
        }
    }

}

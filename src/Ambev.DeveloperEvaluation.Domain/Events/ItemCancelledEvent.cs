namespace Domain.Events
{
    public class ItemCancelledEvent : DomainEvent
    {
        public Guid SaleId { get; }
        public Guid ItemId { get; }
        public Guid ProductId { get; }

        public ItemCancelledEvent(Guid saleId, Guid itemId, Guid productId)
        {
            SaleId = saleId;
            ItemId = itemId;
            ProductId = productId;
        }
    }
}

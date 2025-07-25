namespace Domain.Entities
{
    public class SaleItem
    {
        public Guid Id { get; private set; }
        public Product Product { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal DiscountPercentage { get; private set; }
        public decimal DiscountAmount { get; private set; }
        public decimal SubTotal { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsCancelled { get; private set; }

        protected SaleItem() { } // EF Constructor

        public SaleItem(Product product, int quantity, decimal unitPrice, decimal discountPercentage)
        {
            Id = Guid.NewGuid();
            Product = product ?? throw new ArgumentNullException(nameof(product));

            Update(quantity, unitPrice, discountPercentage);
        }

        public void Update(int quantity, decimal unitPrice, decimal discountPercentage)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot update cancelled item");

            Quantity = quantity;
            UnitPrice = unitPrice;
            DiscountPercentage = discountPercentage;

            CalculateAmounts();
        }

        public void Cancel()
        {
            IsCancelled = true;
            TotalAmount = 0;
        }

        private void CalculateAmounts()
        {
            SubTotal = Quantity * UnitPrice;
            DiscountAmount = SubTotal * DiscountPercentage;
            TotalAmount = SubTotal - DiscountAmount;
        }
    }
}

using Domain.Entities;
using Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Entities
{
    public class Sale
    {
        private readonly List<SaleItem> _items = new();
        private readonly List<IDomainEvent> _domainEvents = new();

        public Guid Id { get; private set; }
        public string SaleNumber { get; private set; }
        public DateTime SaleDate { get; private set; }
        public Customer Customer { get; private set; }
        public Branch Branch { get; private set; }
        public decimal TotalAmount { get; private set; }
        public bool IsCancelled { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public IReadOnlyList<SaleItem> Items => _items.AsReadOnly();
        public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected Sale() { } // EF Constructor

        public Sale(string saleNumber, DateTime saleDate, Customer customer, Branch branch)
        {
            Id = Guid.NewGuid();
            SaleNumber = saleNumber ?? throw new ArgumentNullException(nameof(saleNumber));
            SaleDate = saleDate;
            Customer = customer ?? throw new ArgumentNullException(nameof(customer));
            Branch = branch ?? throw new ArgumentNullException(nameof(branch));
            IsCancelled = false;
            CreatedAt = DateTime.UtcNow;

            AddDomainEvent(new SaleCreatedEvent(Id, SaleNumber, Customer.Id, 0));
        }

        public void AddItem(Product product, int quantity, decimal unitPrice)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot add items to a cancelled sale");

            ValidateQuantity(quantity);

            var discount = CalculateDiscount(quantity);
            var saleItem = new SaleItem(product, quantity, unitPrice, discount);

            _items.Add(saleItem);
            RecalculateTotalAmount();

            AddDomainEvent(new SaleModifiedEvent(Id, TotalAmount));
        }

        public void RemoveItem(Guid itemId)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot remove items from a cancelled sale");

            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new ArgumentException("Item not found");

            item.Cancel();
            RecalculateTotalAmount();

            AddDomainEvent(new ItemCancelledEvent(Id, itemId, item.Product.Id));
            AddDomainEvent(new SaleModifiedEvent(Id, TotalAmount));
        }

        public void UpdateItem(Guid itemId, int newQuantity, decimal newUnitPrice)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot update items in a cancelled sale");

            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item == null)
                throw new ArgumentException("Item not found");

            ValidateQuantity(newQuantity);

            var discount = CalculateDiscount(newQuantity);
            item.Update(newQuantity, newUnitPrice, discount);

            RecalculateTotalAmount();
            AddDomainEvent(new SaleModifiedEvent(Id, TotalAmount));
        }

        public void Cancel()
        {
            if (IsCancelled)
                throw new InvalidOperationException("Sale is already cancelled");

            IsCancelled = true;
            TotalAmount = 0;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new SaleCancelledEvent(Id, SaleNumber));
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        private void ValidateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            if (quantity > 20)
                throw new InvalidOperationException("Cannot sell more than 20 identical items");
        }

        private decimal CalculateDiscount(int quantity)
        {
            return quantity switch
            {
                >= 10 and <= 20 => 0.20m, // 20% discount
                >= 4 => 0.10m,            // 10% discount
                _ => 0m                   // No discount
            };
        }

        private void RecalculateTotalAmount()
        {
            TotalAmount = _items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);
            UpdatedAt = DateTime.UtcNow;
        }

        private void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
    }
}
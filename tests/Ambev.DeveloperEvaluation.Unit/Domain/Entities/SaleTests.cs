using DeveloperStore.Sales.Domain.Entities;
using Domain.Entities;
using Domain.Events;
using FluentAssertions;
using Xunit;

namespace Domain.Tests
{
    public class SaleTests
    {
        private readonly Customer _testCustomer = new()
        {
            Id = Guid.NewGuid(),
            Name = "João Silva",
            Email = "joao@email.com",
            Document = "12345678901"
        };

        private readonly Branch _testBranch = new()
        {
            Id = Guid.NewGuid(),
            Name = "Loja Centro",
            Address = "Rua Principal, 123",
            City = "São Paulo",
            State = "SP"
        };

        private readonly Product _testProduct = new()
        {
            Id = Guid.NewGuid(),
            Name = "Notebook Dell",
            Description = "Notebook Dell Inspiron",
            Category = "Eletrônicos",
            Sku = "DELL001"
        };

        [Fact]
        public void Sale_Constructor_ShouldCreateValidSale()
        {
            // Arrange
            var saleNumber = "SALE-001";
            var saleDate = DateTime.Now;

            // Act
            var sale = new Sale(saleNumber, saleDate, _testCustomer, _testBranch);

            // Assert
            sale.Id.Should().NotBeEmpty();
            sale.SaleNumber.Should().Be(saleNumber);
            sale.SaleDate.Should().Be(saleDate);
            sale.Customer.Should().Be(_testCustomer);
            sale.Branch.Should().Be(_testBranch);
            sale.TotalAmount.Should().Be(0);
            sale.IsCancelled.Should().BeFalse();
            sale.Items.Should().BeEmpty();
            sale.DomainEvents.Should().HaveCount(1);
            sale.DomainEvents.First().Should().BeOfType<SaleCreatedEvent>();
        }

        [Fact]
        public void AddItem_WithValidQuantity_ShouldAddItemAndCalculateDiscount()
        {
            // Arrange
            var sale = CreateTestSale();
            var quantity = 5; // Should get 10% discount
            var unitPrice = 100m;

            // Act
            sale.AddItem(_testProduct, quantity, unitPrice);

            // Assert
            sale.Items.Should().HaveCount(1);

            var item = sale.Items.First();
            item.Quantity.Should().Be(quantity);
            item.UnitPrice.Should().Be(unitPrice);
            item.DiscountPercentage.Should().Be(0.10m); // 10% discount
            item.SubTotal.Should().Be(500m); // 5 * 100
            item.DiscountAmount.Should().Be(50m); // 500 * 0.10
            item.TotalAmount.Should().Be(450m); // 500 - 50

            sale.TotalAmount.Should().Be(450m);
        }

        [Fact]
        public void AddItem_WithQuantityBetween10And20_ShouldGet20PercentDiscount()
        {
            // Arrange
            var sale = CreateTestSale();
            var quantity = 15;
            var unitPrice = 100m;

            // Act
            sale.AddItem(_testProduct, quantity, unitPrice);

            // Assert
            var item = sale.Items.First();
            item.DiscountPercentage.Should().Be(0.20m); // 20% discount
            item.TotalAmount.Should().Be(1200m); // (15 * 100) - (1500 * 0.20)
        }

        [Fact]
        public void AddItem_WithQuantityLessThan4_ShouldNotGetDiscount()
        {
            // Arrange
            var sale = CreateTestSale();
            var quantity = 3;
            var unitPrice = 100m;

            // Act
            sale.AddItem(_testProduct, quantity, unitPrice);

            // Assert
            var item = sale.Items.First();
            item.DiscountPercentage.Should().Be(0m); // No discount
            item.TotalAmount.Should().Be(300m); // 3 * 100
        }

        [Fact]
        public void AddItem_WithQuantityAbove20_ShouldThrowException()
        {
            // Arrange
            var sale = CreateTestSale();

            // Act & Assert
            Action act = () => sale.AddItem(_testProduct, 21, 100m);
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Cannot sell more than 20 identical items");
        }

        [Fact]
        public void AddItem_ToCancelledSale_ShouldThrowException()
        {
            // Arrange
            var sale = CreateTestSale();
            sale.Cancel();

            // Act & Assert
            Action act = () => sale.AddItem(_testProduct, 5, 100m);
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Cannot add items to a cancelled sale");
        }

        [Fact]
        public void RemoveItem_ExistingItem_ShouldCancelItemAndRecalculateTotal()
        {
            // Arrange
            var sale = CreateTestSale();
            sale.AddItem(_testProduct, 5, 100m);
            var itemId = sale.Items.First().Id;

            // Act
            sale.RemoveItem(itemId);

            // Assert
            var item = sale.Items.First();
            item.IsCancelled.Should().BeTrue();
            item.TotalAmount.Should().Be(0);
            sale.TotalAmount.Should().Be(0);
        }

        [Fact]
        public void Cancel_Sale_ShouldSetCancelledAndZeroTotal()
        {
            // Arrange
            var sale = CreateTestSale();
            sale.AddItem(_testProduct, 5, 100m);

            // Act
            sale.Cancel();

            // Assert
            sale.IsCancelled.Should().BeTrue();
            sale.TotalAmount.Should().Be(0);
            sale.DomainEvents.Should().Contain(e => e is SaleCancelledEvent);
        }

        private Sale CreateTestSale()
        {
            return new Sale("SALE-001", DateTime.Now, _testCustomer, _testBranch);
        }
    }

}

using Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Domain.Tests
{
    public class SaleItemTests
    {
        private readonly Product _testProduct = new()
        {
            Id = Guid.NewGuid(),
            Name = "Test Product",
            Description = "Test Description",
            Category = "Test Category",
            Sku = "TEST001"
        };

        [Fact]
        public void SaleItem_Constructor_ShouldCalculateAmountsCorrectly()
        {
            // Arrange
            var quantity = 10;
            var unitPrice = 50m;
            var discountPercentage = 0.15m; // 15%

            // Act
            var item = new SaleItem(_testProduct, quantity, unitPrice, discountPercentage);

            // Assert
            item.Quantity.Should().Be(quantity);
            item.UnitPrice.Should().Be(unitPrice);
            item.DiscountPercentage.Should().Be(discountPercentage);
            item.SubTotal.Should().Be(500m); // 10 * 50
            item.DiscountAmount.Should().Be(75m); // 500 * 0.15
            item.TotalAmount.Should().Be(425m); // 500 - 75
            item.IsCancelled.Should().BeFalse();
        }

        [Fact]
        public void Update_ValidValues_ShouldRecalculateAmounts()
        {
            // Arrange
            var item = new SaleItem(_testProduct, 5, 100m, 0.10m);

            // Act
            item.Update(8, 75m, 0.20m);

            // Assert
            item.Quantity.Should().Be(8);
            item.UnitPrice.Should().Be(75m);
            item.DiscountPercentage.Should().Be(0.20m);
            item.SubTotal.Should().Be(600m); // 8 * 75
            item.DiscountAmount.Should().Be(120m); // 600 * 0.20
            item.TotalAmount.Should().Be(480m); // 600 - 120
        }

        [Fact]
        public void Cancel_ShouldSetCancelledAndZeroTotal()
        {
            // Arrange
            var item = new SaleItem(_testProduct, 5, 100m, 0.10m);

            // Act
            item.Cancel();

            // Assert
            item.IsCancelled.Should().BeTrue();
            item.TotalAmount.Should().Be(0);
        }
    }
}

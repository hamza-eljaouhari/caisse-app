using System;
using System.Collections.Generic;
using Xunit;

namespace CaisseApp.Tests
{
    public class DiscountTests
    {
        [Fact]
        public void ApplyPercentageDiscount_ShouldApplyDiscountCorrectly()
        {
            // Arrange
            var originalPrice = 100;
            var discount = new PercentageDiscount(20);

            // Act
            var discountedPrice = discount.ApplyDiscount(originalPrice);

            // Assert
            Assert.Equal(80, discountedPrice);
        }

        [Fact]
        public void ApplyBOGODiscount_ShouldApplyDiscountCorrectly()
        {
            // Arrange
            var originalPrice = 50;
            var discount = new BOGODiscount(1, 1);

            // Act
            var discountedPrice = discount.ApplyDiscount(originalPrice);

            // Assert
            Assert.Equal(625, discountedPrice);
        }

        [Fact]
        public void ApplyFixedAmountDiscount_ShouldApplyDiscountCorrectly()
        {
            // Arrange
            var originalPrice = 100;
            var discount = new FixedAmountDiscount(10);

            // Act
            var discountedPrice = discount.ApplyDiscount(originalPrice);

            // Assert
            Assert.Equal(90, discountedPrice);
        }

        [Fact]
        public void ApplyFreeShippingDiscount_ShouldReturnZero()
        {
            // Arrange
            var originalPrice = 100;
            var discount = new FreeShippingDiscount();

            // Act
            var discountedPrice = discount.ApplyDiscount(originalPrice);

            // Assert
            Assert.Equal(0, discountedPrice);
        }

        [Fact]
        public void ApplyBTGOFDiscount_ShouldApplyDiscountCorrectly()
        {
            // Arrange
            var originalPrice = 150;
            var discount = new BTGOFDiscount();

            // Act
            var discountedPrice = discount.ApplyDiscount(originalPrice);

            // Assert
            Assert.Equal(100, discountedPrice);
        }
    }
}

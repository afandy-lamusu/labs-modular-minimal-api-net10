using FluentAssertions;
using ModularStore.Api.Modules.Products.Domain;

namespace ModularStore.Tests;

public class ProductTests
{
    [Fact]
    public void Constructor_WhenPriceIsNegative_ThrowsArgumentException()
    {
        // Arrange & Act
        var act = () => new Product("Coffee", "Organic", -10.0m, 10);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*price*");
    }

    [Theory]
    [InlineData(100, 150, false)] // 50% hike — allowed
    [InlineData(100, 151, true)]  // >50% hike — rejected
    public void UpdatePrice_WhenHikeExceeds50Percent_ThrowsException(
        decimal oldPrice, decimal newPrice, bool shouldThrow)
    {
        var product = new Product("Test", "Desc", oldPrice, 10);

        var act = () => product.UpdatePrice(newPrice);

        if (shouldThrow)
            act.Should().Throw<ArgumentException>();
        else
            act.Should().NotThrow();
    }
}

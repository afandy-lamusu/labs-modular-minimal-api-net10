using FluentAssertions;
using NSubstitute;
using ModularStore.Api.Modules.Orders.Application;
using ModularStore.Api.Modules.Orders.Domain;
using ModularStore.Api.Common.Contracts;

namespace ModularStore.Tests;

public class OrderServiceTests
{
    private readonly IProductModule _productModule = Substitute.For<IProductModule>();
    private readonly IOrderRepository _orderRepository = Substitute.For<IOrderRepository>();
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        _service = new OrderService(_productModule, _orderRepository);
    }

    [Fact]
    public async Task PlaceOrder_WhenProductExists_CreatesAndSavesOrder()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productModule.GetProductDetailsAsync(productId, default)
            .Returns(new ProductContractResponse(productId, "Coffee", 5.0m));

        _productModule.DeductStockAsync(productId, 3, default)
            .Returns(true);

        // Act
        var orderId = await _service.PlaceOrderAsync(productId, 3, CancellationToken.None);

        // Assert
        orderId.Should().NotBeEmpty();
        await _orderRepository.Received(1).AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
        await _orderRepository.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task PlaceOrder_WhenStockDeductionFails_ThrowsInvalidOperationException()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _productModule.GetProductDetailsAsync(productId, default)
            .Returns(new ProductContractResponse(productId, "Coffee", 5.0m));

        _productModule.DeductStockAsync(productId, Arg.Any<int>(), default)
            .Returns(false);

        // Act
        var act = async () => await _service.PlaceOrderAsync(productId, 100, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
           .WithMessage("*deduct stock*");
    }

    [Fact]
    public async Task PlaceOrder_WhenProductNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        _productModule.GetProductDetailsAsync(Arg.Any<Guid>(), default)
            .Returns((ProductContractResponse?)null);

        // Act
        var act = async () => await _service.PlaceOrderAsync(Guid.NewGuid(), 1, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}

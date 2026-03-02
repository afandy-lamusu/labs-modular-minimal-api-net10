using FluentAssertions;
using NetArchTest.Rules;
using ModularStore.Api.Modules.Orders.Application;
using ModularStore.Api.Modules.Orders.Domain;

namespace ModularStore.Tests;

public class ArchitectureTests
{
    [Fact]
    public void OrdersModule_ShouldNot_ReferenceProductsInfrastructure()
    {
        var result = Types
            .InAssembly(typeof(OrderService).Assembly)
            .That().ResideInNamespace("ModularStore.Api.Modules.Orders")
            .ShouldNot().HaveDependencyOn("ModularStore.Api.Modules.Products.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(because: "Orders must never reference Products internals");
    }

    [Fact]
    public void DomainLayer_ShouldNot_DependOnInfrastructure()
    {
        var result = Types
            .InAssembly(typeof(Order).Assembly)
            .That().ResideInNamespace("*.Domain")
            .ShouldNot().HaveDependencyOn("*.Infrastructure")
            .GetResult();

        result.IsSuccessful.Should().BeTrue(because: "Domain must stay free of infrastructure concerns");
    }
}

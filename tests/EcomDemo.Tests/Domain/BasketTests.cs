using EcomDemo.Domain.Baskets;
using FluentAssertions;

namespace EcomDemo.Tests.Domain;

public class BasketTests
{
    [Fact]
    public void AddItem_creates_new_line_when_product_not_in_basket()
    {
        var basket = new Basket(Guid.NewGuid());

        basket.AddItem(1, 2);
        basket.AddItem(2, 3);

        basket.Items.Should().HaveCount(2);
        basket.Items.Should().Contain(i => i.ProductId == 1 && i.Quantity == 2);
        basket.Items.Should().Contain(i => i.ProductId == 2 && i.Quantity == 3);
    }

    [Fact]
    public void AddItem_merges_quantity_when_product_already_in_basket()
    {
        var basket = new Basket(Guid.NewGuid());

        basket.AddItem(1, 2);
        basket.AddItem(1, 3);

        basket.Items.Should().HaveCount(1);
        basket.Items.First().ProductId.Should().Be(1);
        basket.Items.First().Quantity.Should().Be(5);
    }
}
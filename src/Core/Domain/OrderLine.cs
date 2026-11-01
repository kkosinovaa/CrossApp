
using Core.Dto;

namespace Core.Domain;

public sealed class OrderLine
{
    public string ProductId { get; }
    public string Name { get; }
    public decimal Price { get; }
    public int Quantity { get; }
    public decimal Total => Price * Quantity;

    private OrderLine(string productId, string name, decimal price, int quantity)
    {
        ProductId = productId;
        Name = name;
        Price = price;
        Quantity = quantity;
    }

    public static OrderLine Create(string productId, string name, decimal price, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productId))
            throw new ArgumentException("ID товару не може бути порожнім");

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва товару не може бути порожньою");

        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Ціна не може бути від'ємною");

        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Кількість має бути більшою за нуль");

        return new OrderLine(productId.Trim(), name.Trim(), price, quantity);
    }

    public OrderLineDto ToDto(string orderId)
    {
        return new OrderLineDto(orderId, ProductId, Quantity, Name, Price);
    }

    public static OrderLine FromDto(OrderLineDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        if (dto.Name is null || dto.Price is null)
            throw new ArgumentException("У DTO відсутні назва або ціна товару");

        return Create(dto.ProductId, dto.Name, dto.Price.Value, dto.Quantity);
    }
}

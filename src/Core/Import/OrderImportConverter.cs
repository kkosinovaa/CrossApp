
using Core.Domain;
using Core.Dto;

namespace Core.Import;

public static class OrderImportConverter
{
    public static ImportResult<Order> Convert(ImportResult<OrderDomainDto> source)
    {
        ArgumentNullException.ThrowIfNull(source);

        List<string> errors = new(source.Errors);
        Dictionary<string, Order> orders = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, ProductDto> products = new(StringComparer.OrdinalIgnoreCase);

        foreach (ProductDto dto in source.Items.OfType<ProductDto>())
        {
            if (string.IsNullOrWhiteSpace(dto.Id) ||
                string.IsNullOrWhiteSpace(dto.Name) ||
                dto.Price < 0)
            {
                errors.Add($"Товар {dto.Id}: некоректні дані");
                continue;
            }

            if (!products.TryAdd(dto.Id, dto))
                errors.Add($"Товар {dto.Id}: повторний ID");
        }

        foreach (OrderDto dto in source.Items.OfType<OrderDto>())
        {
            try
            {
                Order order = Order.FromDto(dto);

                if (!orders.TryAdd(order.Id, order))
                    errors.Add($"Замовлення {order.Id}: повторний ID");
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                errors.Add($"Замовлення {dto.Id}: {ex.Message}");
            }
        }

        foreach (OrderLineDto dto in source.Items.OfType<OrderLineDto>())
        {
            if (string.IsNullOrWhiteSpace(dto.OrderId) ||
                !orders.TryGetValue(dto.OrderId, out Order? order))
            {
                errors.Add($"Рядок: замовлення {dto.OrderId} не знайдено");
                continue;
            }

            string name;
            decimal price;

            if (dto.Name is not null && dto.Price.HasValue)
            {
                name = dto.Name;
                price = dto.Price.Value;
            }
            else if (!string.IsNullOrWhiteSpace(dto.ProductId) &&
                     products.TryGetValue(dto.ProductId, out ProductDto? product))
            {
                name = product.Name;
                price = product.Price;
            }
            else
            {
                errors.Add($"Рядок замовлення {dto.OrderId}: товар {dto.ProductId} не знайдено");
                continue;
            }

            try
            {
                order.AddLine(dto.ProductId, name, price, dto.Quantity);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
            {
                errors.Add($"Рядок замовлення {dto.OrderId}: {ex.Message}");
            }
        }

        return new ImportResult<Order>(orders.Values.ToList(), errors);
    }
}

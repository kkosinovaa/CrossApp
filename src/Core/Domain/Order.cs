using System.Globalization;
using Core.Dto;

namespace Core.Domain;

public sealed class Order
{
    private readonly List<OrderLine> _lines = [];

    public string Id { get; }
    public string CustomerId { get; }
    public string Date { get; }
    public OrderStatus Status { get; private set; } = OrderStatus.Draft;
    public bool IsConfirmed => Status == OrderStatus.Confirmed;
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    public decimal Total => _lines.Sum(line => line.Total);

    private Order(string id, string customerId, string date)
    {
        Id = id;
        CustomerId = customerId;
        Date = date;
    }

    public static Order Create(string id, string customerId, string date)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID замовлення не може бути порожнім", nameof(id));

        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentException("ID клієнта не може бути порожнім", nameof(customerId));

        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd",
            CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            throw new ArgumentException("Некоректна дата замовлення", nameof(date));

        return new Order(id.Trim(), customerId.Trim(), date);
    }

    public void AddLine(string productId, string name, decimal price, int quantity)
    {
        if (Status != OrderStatus.Draft)
            throw new InvalidOperationException($"Не можна змінювати замовлення зі статусом {Status}");

        _lines.Add(OrderLine.Create(productId, name, price, quantity));
    }

    public void ChangeStatus(OrderStatus nextStatus)
    {
        bool allowed = (Status, nextStatus) switch
        {
            (OrderStatus.Draft, OrderStatus.Confirmed) when _lines.Count > 0 => true,
            (OrderStatus.Draft, OrderStatus.Cancelled) => true,
            (OrderStatus.Confirmed, OrderStatus.Cancelled) => true,
            _ => false
        };

        if (!allowed)
        {
            if (Status == OrderStatus.Draft &&
                nextStatus == OrderStatus.Confirmed &&
                _lines.Count == 0)
                throw new InvalidOperationException("Не можна підтвердити порожнє замовлення");

            throw new InvalidOperationException($"Перехід {Status} -> {nextStatus} заборонений");
        }

        Status = nextStatus;
    }

    public void Confirm() => ChangeStatus(OrderStatus.Confirmed);

    public void Cancel() => ChangeStatus(OrderStatus.Cancelled);

    public OrderDto ToDto()
    {
        return new OrderDto(
            Id,
            CustomerId,
            Date,
            IsConfirmed,
            _lines.Select(line => line.ToDto(Id)).ToList(),
            Status.ToString()
        );
    }

    public static Order FromDto(OrderDto dto)
    {
        ArgumentNullException.ThrowIfNull(dto);

        Order order = Create(dto.Id, dto.CustomerId, dto.Date);

        foreach (OrderLineDto lineDto in dto.Lines ?? [])
        {
            if (!string.Equals(lineDto.OrderId, order.Id, StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("ID замовлення у рядку не збігається");

            OrderLine line = OrderLine.FromDto(lineDto);
            order.AddLine(line.ProductId, line.Name, line.Price, line.Quantity);
        }

        OrderStatus savedStatus;

        if (string.IsNullOrWhiteSpace(dto.Status))
        {
            savedStatus = dto.IsConfirmed ? OrderStatus.Confirmed : OrderStatus.Draft;
        }
        else if (!Enum.TryParse(dto.Status, true, out savedStatus) ||
                 !Enum.IsDefined(savedStatus))
        {
            throw new ArgumentException($"Невідомий статус замовлення: {dto.Status}");
        }

        if (dto.Status is not null &&
            dto.IsConfirmed != (savedStatus == OrderStatus.Confirmed))
            throw new ArgumentException("Статус і ознака підтвердження у DTO не збігаються");

        if (savedStatus == OrderStatus.Confirmed)
            order.Confirm();
        else if (savedStatus == OrderStatus.Cancelled)
            order.Cancel();

        return order;
    }

    public override string ToString()
    {
        return $"Замовлення {Id}, клієнт {CustomerId}, дата {Date}, сума {Total:F2} грн, статус: {Status}";
    }
}

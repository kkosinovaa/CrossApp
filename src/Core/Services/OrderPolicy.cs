using Core.Domain;

namespace Core.Services;

public static class OrderPolicy
{
    public static Order CreateForCustomer(
        Customer customer,
        IEnumerable<Order> existingOrders,
        string id,
        string date)
    {
        ArgumentNullException.ThrowIfNull(customer);
        ArgumentNullException.ThrowIfNull(existingOrders);

        Order order = Order.Create(id, customer.Id, date);
        List<Order> orders = existingOrders.ToList();

        if (orders.Any(o => string.Equals(o.Id, id, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Замовлення {id} вже існує");

        int activeCount = orders.Count(o =>
            string.Equals(o.CustomerId, customer.Id, StringComparison.OrdinalIgnoreCase)
            && o.Status != OrderStatus.Cancelled);

        if (activeCount >= 5)
            throw new InvalidOperationException($"Клієнт {customer.Id} вже має 5 активних замовлень");

        return order;
    }
}

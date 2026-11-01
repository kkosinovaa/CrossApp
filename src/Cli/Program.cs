
using Core;
using Core.Domain;
using Core.Dto;
using Core.Import;
using Core.Services;

if (args.Length > 0 && args[0] == "--lab04")
{
    RunLab04();
    return;
}

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return;
}

ImportResult<OrderDomainDto> result = Path.GetExtension(path).ToLowerInvariant() switch
{
    ".csv" => OrderCsvImporter.Load(path),
    ".json" => OrderJsonImporter.Load(path),
    var ext => new ImportResult<OrderDomainDto>([], [$"Непідтримуваний формат: {ext}"])
};

int accepted = result.Items.Count;
int skipped = result.Errors.Count;
int total = accepted + skipped;
double errorRate = total > 0 ? (double)skipped / total * 100 : 0;

Console.WriteLine($"Статистика імпорту: усього {total} | прийнято {accepted} | пропущено {skipped} | помилок {errorRate:F1}%");

foreach (OrderDomainDto item in result.Items.Take(5))
{
    switch (item)
    {
        case CustomerDto c:
            Console.WriteLine($"[Клієнт] {c.Id} {c.Name} {c.Phone}");
            break;
        case ProductDto p:
            Console.WriteLine($"[Товар] {p.Id} {p.Name} {p.Price:F2} грн");
            break;
        case OrderDto o:
            Console.WriteLine($"[Замовлення] {o.Id} Клієнт: {o.CustomerId} Дата: {o.Date}");
            break;
        case OrderLineDto l:
            Console.WriteLine($"[Рядок] Зам: {l.OrderId} Товар: {l.ProductId} К-ть: {l.Quantity}");
            break;
    }
}

foreach (string error in result.Errors)
    Console.WriteLine($"Помилка: {error}");

static void RunLab04()
{
    Console.WriteLine("=== Лабораторна робота 4 ===");
    Console.WriteLine("=== Сценарій 1: успішні операції ===");

    Order order = Order.Create("O-101", "C-001", "2026-11-01");

    order.AddLine("P-001", "Ноутбук", 25000m, 1);
    order.AddLine("P-002", "Мишка", 750m, 2);

    Console.WriteLine(order);

    foreach (OrderLine line in order.Lines)
        Console.WriteLine($"{line.Name}: {line.Quantity} шт. x {line.Price:F2} = {line.Total:F2} грн");

    order.Confirm();
    Console.WriteLine("Замовлення підтверджено");
    Console.WriteLine(order);

    OrderDto dto = order.ToDto();
    Order restored = Order.FromDto(dto);

    Console.WriteLine("Відновлено з DTO:");
    Console.WriteLine(restored);

    Console.WriteLine();
    Console.WriteLine("=== Сценарій 2: порушення інваріантів ===");

    TryDo("Порожній ID", () => Order.Create("", "C-001", "2026-11-01"));
    TryDo("Некоректна дата", () => Order.Create("O-102", "C-001", "bad-date"));
    TryDo("Від'ємна ціна", () => OrderLine.Create("P-003", "Телефон", -100m, 1));
    TryDo("Нульова кількість", () => OrderLine.Create("P-003", "Телефон", 100m, 0));
    TryDo("Зміна підтвердженого замовлення", () => order.AddLine("P-003", "Телефон", 100m, 1));
    TryDo("Повторне підтвердження", () => order.Confirm());

    Order emptyOrder = Order.Create("O-103", "C-002", "2026-11-01");
    TryDo("Підтвердження порожнього замовлення", () => emptyOrder.Confirm());

    Console.WriteLine("Стан після помилок:");
    Console.WriteLine(order);
    Console.WriteLine($"Кількість рядків: {order.Lines.Count}");

    Console.WriteLine();
    Console.WriteLine("=== Додаткове завдання 1 ===");

    var imported = OrderCsvImporter.Load("data/sample.csv");
    var items = imported.Items.ToList();

    items.Add(new OrderDto("O-BAD", "C-001", "wrong-date"));
    items.Add(new OrderLineDto("O-001", "P-001", 0));

    var converted = OrderImportConverter.Convert(
        new ImportResult<OrderDomainDto>(items, imported.Errors));

    Console.WriteLine($"Коректних замовлень: {converted.Items.Count}");
    Console.WriteLine($"Помилок: {converted.Errors.Count}");

    foreach (string error in converted.Errors)
        Console.WriteLine(error);

    Console.WriteLine();
    Console.WriteLine("=== Додаткове завдання 2 ===");

    Customer customer = Customer.Create("C-777", "Марія");
    List<Order> orders = [];

    for (int i = 1; i <= 5; i++)
    {
        Order newOrder = OrderPolicy.CreateForCustomer(
            customer, orders, $"O-{i}", "2026-11-01");

        orders.Add(newOrder);
    }

    Console.WriteLine($"Створено замовлень: {orders.Count}");

    TryDo("Шосте замовлення", () =>
        OrderPolicy.CreateForCustomer(
            customer, orders, "O-6", "2026-11-01"));

    orders[0].Cancel();

    Order sixthOrder = OrderPolicy.CreateForCustomer(
        customer, orders, "O-6", "2026-11-01");

    Console.WriteLine($"Після скасування створено: {sixthOrder.Id}");

    Console.WriteLine();
    Console.WriteLine("=== Додаткове завдання 3 ===");

    Order testOrder = Order.Create("O-200", "C-001", "2026-11-01");

    Console.WriteLine($"Початковий статус: {testOrder.Status}");

    testOrder.AddLine("P-001", "Ноутбук", 25000m, 1);
    testOrder.Confirm();

    Console.WriteLine($"Після підтвердження: {testOrder.Status}");

    testOrder.Cancel();

    Console.WriteLine($"Після скасування: {testOrder.Status}");

    TryDo("Повернення до Draft", () =>
        testOrder.ChangeStatus(OrderStatus.Draft));

    Order restoredOrder = Order.FromDto(testOrder.ToDto());
    Console.WriteLine($"Статус після відновлення: {restoredOrder.Status}");
}

static void TryDo(string title, Action action)
{
    try
    {
        action();
        Console.WriteLine($"{title}: помилка НЕ спрацювала");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"{title}: {ex.GetType().Name} - {ex.Message}");
    }
}

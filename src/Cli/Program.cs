using Core;
using Core.Dto;
using Core.Import;

string path = args.Length > 0 ? args[0] : Path.Combine("data", "sample.csv");

if (!File.Exists(path))
{
    Console.WriteLine($"Файл не знайдено: {Path.GetFullPath(path)}");
    return 1;
}

ImportResult<OrderDomainDto> result = OrderCsvImporter.Load(path);
Console.WriteLine($"Завантажено записів: {result.Items.Count}");

// Виводимо перші 5 записів, перевіряючи конкретний тип через pattern matching
foreach (OrderDomainDto item in result.Items.Take(5))
{
    switch (item)
    {
        case CustomerDto c:
            Console.WriteLine($" [Клієнт] {c.Id,-6} {c.Name,-15} {c.Phone}");
            break;
        case ProductDto p:
            Console.WriteLine($" [Товар]  {p.Id,-6} {p.Name,-15} {p.Price,8:F2} грн");
            break;
        case OrderDto o:
            Console.WriteLine($" [Замовл] {o.Id,-6} Клієнт: {o.CustomerId,-7} Дата: {o.Date}");
            break;
        case OrderLineDto l:
            Console.WriteLine($" [Рядок]  Зам: {l.OrderId,-5} Товар: {l.ProductId,-5} К-ть: {l.Quantity}");
            break;
    }
}

if (result.Errors.Count > 0)
{
    Console.WriteLine($"\nПропущено рядків: {result.Errors.Count}");
    foreach (string e in result.Errors)
    {
        Console.WriteLine($" ! {e}");
    }
}

return 0;
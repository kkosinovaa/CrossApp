using System.Globalization;
using Core.Dto;

namespace Core.Import;

public static class OrderCsvImporter
{
    private const char Separator = ';';

    public static ImportResult<OrderDomainDto> Load(string path)
    {
        var items = new List<OrderDomainDto>();
        var errors = new List<string>();
        string[] lines = File.ReadAllLines(path);

        for (int i = 0; i < lines.Length; i++)
        {
            int number = i + 1;
            string line = lines[i];

            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            // Пропускаємо рядок заголовків
            if (number == 1 && line.StartsWith("Type", StringComparison.OrdinalIgnoreCase))
                continue;

            switch (ParseLine(line))
            {
                case ParseOk ok:
                    items.Add(ok.Value);
                    break;
                case ParseFailed failed:
                    errors.Add($"рядок {number}: {failed.Reason}");
                    break;
            }
        }
        return new ImportResult<OrderDomainDto>(items, errors);
    }

    private static ParseOutcome ParseLine(string line)
    {
        string[] parts = line.Split(Separator, StringSplitOptions.TrimEntries);

        return parts switch
        {
            { Length: < 3 } => new ParseFailed($"Замало колонок, отримав {parts.Length}"),

            // Патерни для Клієнта (Customer)
            ["C", _, "", ..] => new ParseFailed("Ім'я клієнта порожнє"),
            ["C", var id, var name] => new ParseOk(new CustomerDto(id, name)),
            ["C", var id, var name, var phone] => new ParseOk(new CustomerDto(id, name, phone)),

            // Патерни для Товару (Product)
            ["P", var id, var name, var priceStr] when decimal.TryParse(priceStr, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) && price >= 0
                => new ParseOk(new ProductDto(id, name, price)),
                ["P", .., var priceStr] => new ParseFailed($"Ціна '{priceStr}' не є коректним невід'ємним числом"),

                // Патерни для Замовлення (Order)
                ["O", var id, var customerId, var date] => new ParseOk(new OrderDto(id, customerId, date)),

                // Патерни для Рядка замовлення (OrderLine)
                ["L", var orderId, var productId, var qtyStr] when int.TryParse(qtyStr, out int qty) && qty > 0
                    => new ParseOk(new OrderLineDto(orderId, productId, qty)),
                    ["L", .., var qtyStr] => new ParseFailed($"Кількість '{qtyStr}' має бути додатним числом"),

                    [var type, ..] => new ParseFailed($"Невідомий префікс типу: '{type}'")
        };
    }

    private abstract record ParseOutcome;
    private sealed record ParseOk(OrderDomainDto Value) : ParseOutcome;
    private sealed record ParseFailed(string Reason) : ParseOutcome;
}
using System.Text.Json;
using Core.Dto;

namespace Core.Import;

public static class OrderJsonImporter
{
    public static ImportResult<OrderDomainDto> Load(string path)
    {
        var errors = new List<string>();
        try
        {
            string json = File.ReadAllText(path);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var items = JsonSerializer.Deserialize<List<OrderDomainDto>>(json, options) ?? [];
            return new ImportResult<OrderDomainDto>(items, errors);
        }
        catch (JsonException ex)
        {
            errors.Add($"Помилка розбору JSON: {ex.Message}");
            return new ImportResult<OrderDomainDto>([], errors);
        }
    }
}
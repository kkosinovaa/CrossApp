namespace Core.Domain;

public sealed class Customer
{
    public string Id { get; }
    public string Name { get; }

    private Customer(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Customer Create(string id, string name)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("ID клієнта не може бути порожнім", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ім'я клієнта не може бути порожнім", nameof(name));

        return new Customer(id.Trim(), name.Trim());
    }
}

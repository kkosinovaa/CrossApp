namespace Core.Dto;

public abstract record OrderDomainDto;

public record CustomerDto(string Id, string Name, string? Phone = null) : OrderDomainDto;

public record ProductDto(string Id, string Name, decimal Price) : OrderDomainDto;

public record OrderDto(string Id, string CustomerId, string Date) : OrderDomainDto;

public record OrderLineDto(string OrderId, string ProductId, int Quantity) : OrderDomainDto;
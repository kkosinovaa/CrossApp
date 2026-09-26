using System.Text.Json.Serialization;

namespace Core.Dto;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(CustomerDto), "C")]
[JsonDerivedType(typeof(ProductDto), "P")]
[JsonDerivedType(typeof(OrderDto), "O")]
[JsonDerivedType(typeof(OrderLineDto), "L")]
public abstract record OrderDomainDto;

public record CustomerDto(string Id, string Name, string? Phone = null) : OrderDomainDto;
public record ProductDto(string Id, string Name, decimal Price) : OrderDomainDto;
public record OrderDto(string Id, string CustomerId, string Date) : OrderDomainDto;
public record OrderLineDto(string OrderId, string ProductId, int Quantity) : OrderDomainDto;
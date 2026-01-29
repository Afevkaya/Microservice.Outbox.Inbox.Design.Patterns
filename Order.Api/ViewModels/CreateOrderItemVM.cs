namespace Order.Api.ViewModels;

public record CreateOrderItemVM(Guid ProductId, int Quantity, decimal UnitPrice);
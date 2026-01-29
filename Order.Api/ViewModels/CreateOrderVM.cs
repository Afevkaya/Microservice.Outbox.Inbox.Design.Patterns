namespace Order.Api.ViewModels;

public record CreateOrderVM(Guid BuyerId, List<CreateOrderItemVM> OrderItems);
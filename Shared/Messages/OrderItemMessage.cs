namespace Shared.Messages;

public record OrderItemMessage(Guid ProductId, int Quantity, decimal UnitPrice);
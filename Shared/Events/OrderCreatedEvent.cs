using Shared.Messages;

namespace Shared.Events;

public record OrderCreatedEvent(Guid IdempotentToken, Guid BuyerId, Guid OrderId, decimal TotalPrice, List<OrderItemMessage> OrderItems);
using Shared.Messages;

namespace Shared.Events;

public record OrderCreatedEvent(Guid BuyerId, Guid OrderId, decimal TotalPrice, List<OrderItemMessage> OrderItems);
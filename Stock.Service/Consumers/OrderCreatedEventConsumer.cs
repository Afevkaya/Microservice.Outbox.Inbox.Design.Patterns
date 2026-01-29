using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Events;
using Stock.Service.Contexts;

namespace Stock.Service.Consumers;

public class OrderCreatedEventConsumer(StockDbContext stockDbContext) : IConsumer<OrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
    {
        await stockDbContext.OrderInboxes.AddAsync(new()
        {
            Processed = false,
            Payload = JsonSerializer.Serialize(context.Message)
        });
        await stockDbContext.SaveChangesAsync();
        
        await Task.Delay(10000);
        
        var orderInboxes = await stockDbContext.OrderInboxes.Where(oi=> oi.Processed == false).ToListAsync();
        if(orderInboxes.Count == 0) return;
        foreach (var orderInbox in orderInboxes)
        {
            var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderInbox.Payload);
            if (orderCreatedEvent == null) continue;
            await Console.Out.WriteLineAsync($"OrderCreatedEvent: {orderCreatedEvent!.OrderId} published.");
            orderInbox.Processed = true;
            await stockDbContext.SaveChangesAsync();
        }
    }
}
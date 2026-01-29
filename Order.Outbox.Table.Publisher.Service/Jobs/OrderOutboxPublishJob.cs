using System.Text.Json;
using MassTransit;
using Order.Outbox.Table.Publisher.Service.Entities;
using Quartz;
using Shared.Events;

namespace Order.Outbox.Table.Publisher.Service.Jobs;

[DisallowConcurrentExecution]
public class OrderOutboxPublishJob(IPublishEndpoint publishEndpoint, OrderOutboxDatabase database) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var orderOutboxes = (await database.QueryAsync<OrderOutbox>(
            "SELECT * FROM ORDEROUTBOXES WHERE PROCESSDATE IS NULL ORDER BY OCCUREDON ASC")).ToList();
        
        foreach (var orderOutbox in orderOutboxes)
        {
            if (orderOutbox.Type != nameof(OrderCreatedEvent)) continue;
            var orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);
            if (orderCreatedEvent == null) continue;
            await publishEndpoint.Publish(orderCreatedEvent, context.CancellationToken);
            await database.ExecuteAsync(
                $"UPDATE ORDEROUTBOXES SET PROCESSDATE = GETDATE() WHERE ID = '{orderOutbox.Id}'");
            
            await Console.Out.WriteLineAsync($"OrderOutboxId: {orderOutbox.Id} published.");
        }
    }
}
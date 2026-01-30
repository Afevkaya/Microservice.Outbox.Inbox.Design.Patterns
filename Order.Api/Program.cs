using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Api.Contexts;
using Order.Api.Models.Entities;
using Order.Api.ViewModels;
using Shared.Events;
using Shared.Messages;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) => { cfg.Host(builder.Configuration["RabbitMq"]); });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MapPost("/create-order", async 
    (CreateOrderVM model, OrderDbContext dbContext, ISendEndpointProvider sendEndpointProvider) =>
    {
        Order.Api.Models.Entities.Order order = new()
        {
            BuyerId = model.BuyerId,
            TotalPrice = model.OrderItems.Sum(x => x.UnitPrice * x.Quantity),
            OrderItems = model.OrderItems.Select(oi => new Order.Api.Models.Entities.OrderItem
            {
                ProductId = oi.ProductId,
                Quantity = oi.Quantity,
                UnitPrice = oi.UnitPrice
            }).ToList(),
            CreatedDate = DateTime.UtcNow,
        };

        await dbContext.Orders.AddAsync(order);
        await dbContext.SaveChangesAsync();

        var idempotentToken = Guid.NewGuid();
        
        OrderCreatedEvent orderCreatedEvent = new(idempotentToken, order.BuyerId, order.Id, order.TotalPrice,
            model.OrderItems.Select(oi => new OrderItemMessage(oi.ProductId, oi.Quantity, oi.UnitPrice)).ToList());
        #region Outbox Pattern Olmadan

        // var sendEndpoint =
        //     await sendEndpointProvider.GetSendEndpoint(
        //         new Uri($"queue:{RabbitMqSettings.Stock_OrderCreatedEventQueue}"));
        // await sendEndpoint.Send(orderCreatedEvent);

        #endregion

        #region Outbox Pattern Eklendi

        OrderOutbox orderOutbox = new()
        {
            OccuredOn = DateTime.UtcNow,
            ProcessDate = null,
            Type = orderCreatedEvent.GetType().Name,
            Payload = JsonSerializer.Serialize(orderCreatedEvent),
            IdempotentToken = idempotentToken
        };
        
        await dbContext.OrderOutboxes.AddAsync(orderOutbox);
        await dbContext.SaveChangesAsync();

        #endregion
    });

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();


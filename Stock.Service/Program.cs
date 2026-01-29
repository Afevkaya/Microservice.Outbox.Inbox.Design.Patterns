using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.Service.Consumers;
using Stock.Service.Contexts;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<StockDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection"));
});

builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<OrderCreatedEventConsumer>();
    config.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq"]);
        cfg.ReceiveEndpoint(RabbitMqSettings.Stock_OrderCreatedEventQueue,e =>
        {
            e.ConfigureConsumer<OrderCreatedEventConsumer>(context);
        });
    });
    
});
var app = builder.Build();

app.Run();

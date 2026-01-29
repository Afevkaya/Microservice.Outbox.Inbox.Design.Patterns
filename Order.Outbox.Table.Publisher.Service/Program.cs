
using MassTransit;
using Order.Outbox.Table.Publisher.Service;
using Order.Outbox.Table.Publisher.Service.Jobs;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<OrderOutboxDatabase>();
builder.Services.AddQuartz(configurator =>
{
    configurator.UseMicrosoftDependencyInjectionJobFactory();
    JobKey jobKey = new("OrderOutboxPublishJob");
    configurator.AddJob<OrderOutboxPublishJob>(jobConfigurator =>
    {
        jobConfigurator.WithIdentity(jobKey);
    });

    TriggerKey triggerKey = new("OrderOutboxPublishJobTrigger");
    configurator.AddTrigger(triggerCfg => triggerCfg.ForJob(jobKey)
        .WithIdentity(triggerKey)
        .StartAt(DateTime.UtcNow)
        .WithSimpleSchedule(build=> build
            .WithIntervalInSeconds(5)
            .RepeatForever()));
});

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services.AddMassTransit(config =>
{
    config.UsingRabbitMq((context, cfg) => { cfg.Host(builder.Configuration["RabbitMq"]); });
});
var host = builder.Build();
host.Run();
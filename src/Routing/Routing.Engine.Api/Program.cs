using MassTransit;
using Routing.Engine.Application.Contracts;
using Routing.Engine.Application.Strategy;
using Routing.Engine.Application.UseCases.ChooseProvider;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IProviderRoutingStrategy, AmountBasedRoutingStrategy>();
builder.Services.AddScoped<IChooseProviderUseCase, ChooseProviderUseCase>();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<PaymentRequestedConsumer>();
    x.UsingInMemory((ctx, cfg) => { });
    x.AddRider(r =>
    {
        r.AddConsumer<PaymentRequestedConsumer>();
        r.AddProducer<BuildingBlocks.Messaging.ProviderChosen>("payments.provider.chosen");
        r.UsingKafka(
            (ctx, k) =>
            {
                var bootstrap =
                    builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:19092";
                k.Host("localhost:19092");

                // Endpoint para consumir payments.requested
                k.TopicEndpoint<BuildingBlocks.Messaging.PaymentRequested>(
                    "payments.requested",
                    "routing-engine",
                    e =>
                    {
                        e.ConfigureConsumer<PaymentRequestedConsumer>(ctx);
                        e.UseMessageRetry(r =>
                        {
                            r.Immediate(3);
                        });
                        e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                    }
                );
            }
        );
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

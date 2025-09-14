using MassTransit;
using Providers.Fake.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    // Consumer
    x.AddConsumer<ProviderChosenConsumer>();

    // Bus em memória para DI (não usamos como transporte)
    x.UsingInMemory((ctx, cfg) => { });

    // Kafka Rider
    x.AddRider(r =>
    {
        r.AddConsumer<ProviderChosenConsumer>();
        r.AddProducer<BuildingBlocks.Messaging.ProviderCallback>("payments.provider.callback");

        r.UsingKafka(
            (ctx, k) =>
            {
                var bootstrap =
                    builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:19092";
                k.Host(bootstrap);

                // Consome as decisões do roteador
                k.TopicEndpoint<BuildingBlocks.Messaging.ProviderChosen>(
                    "payments.provider.chosen",
                    "providers-fake",
                    e =>
                    {
                        e.ConfigureConsumer<ProviderChosenConsumer>(ctx);
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

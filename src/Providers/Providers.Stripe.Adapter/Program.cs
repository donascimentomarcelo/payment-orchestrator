using MassTransit;
using Providers.Stripe.Infrastructure.Messaging;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

var apiKey =
    Environment.GetEnvironmentVariable("STRIPE_API_KEY")
    ?? builder.Configuration["Stripe:ApiKey"]
    ?? "";

Console.WriteLine($"[DEBUG] Stripe Key = {apiKey}");

if (string.IsNullOrWhiteSpace(apiKey))
{
    Console.WriteLine("WARNING: STRIPE_API_KEY not set. Set env STRIPE_API_KEY=sk_test_...");
}

builder.Services.AddSingleton(new StripeClient(apiKey));
builder.Services.AddSingleton<PaymentIntentService>();

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<StripeClient>();
    return new PaymentIntentService(client);
});

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.AddConsumer<ProviderChosenConsumer>();

    x.UsingInMemory((ctx, cfg) => { });

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

                // Consome decisões de roteamento
                k.TopicEndpoint<BuildingBlocks.Messaging.ProviderChosen>(
                    "payments.provider.chosen",
                    "providers-stripe",
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

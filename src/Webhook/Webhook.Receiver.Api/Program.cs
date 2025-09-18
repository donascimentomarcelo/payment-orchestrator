using BuildingBlocks.Messaging;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingInMemory((ctx, cfg) => { });

    x.AddRider(r =>
    {
        r.AddProducer<ProviderCallback>("payments.provider.callback");
        r.UsingKafka(
            (ctx, k) =>
            {
                var bootstrap = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
                k.Host(bootstrap);
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

using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payments.Api.Publisher;
using Payments.Domain;
using Payments.Domain.Entities;
using Payments.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


// DB
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentsDb"),
        npg => npg.MigrationsAssembly("Payments.Infrastructure")));

// Kafka
builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();
    x.UsingInMemory((ctx, cfg) => { });
    x.AddRider(rider =>
    {
        rider.AddProducer<PaymentRequested>("payment.requested");
        rider.UsingKafka((ctx, k) =>
        {
            var bootstrap = builder.Configuration["Kafka:BootstrapServers"] ?? "localhost:9092";
            k.Host(bootstrap);
        });
    });
});

builder.Services.AddHostedService<OutboxPublisher>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapPost("/payments", async (PaymentRequest req, PaymentsDbContext db) =>
{
    var payment = new Payment(req.Amount, req.Currency);
    db.Payments.Add(payment);

    var evt = new PaymentRequested(payment.Id, payment.Amount, payment.Currency, DateTime.UtcNow);
    var payload = System.Text.Json.JsonSerializer.Serialize(evt);

    db.Outbox.Add(new OutboxMessage
    {
        Id = Guid.NewGuid(),
        Type = nameof(Payment),
        Payload = payload,
        CreatedAt = DateTime.UtcNow,
        Published = false,
    });

    await db.SaveChangesAsync();
    return Results.Created($"/payments/{payment.Id}", payment);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

record PaymentRequest(decimal Amount, string Currency);
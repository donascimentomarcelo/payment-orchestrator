using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payments.Domain;
using Payments.Domain.Entities;
using Payments.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


// DB
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PaymentsDb")));

// Kafka
builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory(); // fallback local
    x.AddRider(rider =>
    {
        rider.UsingKafka((ctx, k) =>
        {
            k.Host("localhost:9092");
        });
    });
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.MapPost("/payments", async (PaymentRequest req, PaymentsDbContext db) =>
{
    var payment = new Payment(req.Amount, req.Currency);
    db.Payments.Add(payment);

    db.Outbox.Add(new OutboxMessage
    {
        Id = Guid.NewGuid(),
        Type = nameof(Payment),
        Payload = System.Text.Json.JsonSerializer.Serialize(new
        {
            payment.Id,
            payment.Amount,
            payment.Currency,
            payment.Status,
        }),
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
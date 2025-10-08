using BuildingBlocks.Messaging;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payments.Application.Abstractions;
using Payments.Application.UseCases.CapturePayment;
using Payments.Application.UseCases.CreatePayment;
using Payments.Application.UseCases.ProcessCallback;
using Payments.Application.UseCases.RefundPayment;
using Payments.Infrastructure.Messaging;
using Payments.Infrastructure.Outbox;
using Payments.Infrastructure.Persistence;
using Payments.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("PaymentsDb"),
        npg => npg.MigrationsAssembly("Payments.Infrastructure")
    )
);

builder.Services.AddMassTransit(x =>
{
    x.SetKebabCaseEndpointNameFormatter();

    x.AddConsumer<ProviderCallbackConsumer>();

    x.UsingInMemory((ct, cf) => { });
    x.AddRider(rider =>
    {
        rider.AddProducer<PaymentRequested>("payments.requested");
        rider.AddConsumer<ProviderCallbackConsumer>();
        rider.UsingKafka(
            (ctx, k) =>
            {
                k.Host("localhost:19092");
                k.TopicEndpoint<ProviderCallback>(
                    "payments.provider.callback",
                    "payments-api",
                    e =>
                    {
                        e.ConfigureConsumer<ProviderCallbackConsumer>(ctx);
                        e.AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
                    }
                );
            }
        );
    });
});

builder.Services.AddHostedService<OutboxPublisher>();

builder.Services.AddScoped<ICreatePaymentUseCase, CreatePaymentUseCase>();
builder.Services.AddScoped<IPaymentRepository, EfPaymentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOutboxWriter, EfOutboxWriter>();
builder.Services.AddScoped<IProcessProviderCallbackUseCase, ProcessProviderCallbackUseCase>();
builder.Services.AddScoped<ICapturePaymentUseCase, CapturePaymentUseCase>();
builder.Services.AddScoped<IRefundPaymentUseCase, RefundPaymentUseCase>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();

record PaymentRequest(decimal Amount, string Currency);

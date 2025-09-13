
# Run

dotnet run --project src/Payments/Payments.Api

# Criar Tópico Kafka

docker exec -it redpanda rpk topic create payments.requested payments.provider.chosen \
  payments.provider.callback payments.authorized payments.captured payments.failed \
  ledger.entries reconciliation.mismatch checkout.readmodel.updated

# Teste de conectividade 

- docker exec -it redpanda rpk topic list
- docker exec -it postgres sh
- psql -h localhost -U payments -d paymentsdb

# EF Core Migration

- dotnet tool install --global dotnet-ef
- dotnet ef migrations add Init \
  --project src/Payments/Payments.Infrastructure \
  --startup-project src/Payments/Payments.Api \
  -c PaymentsDbContext
- dotnet ef database update \
  --project src/Payments/Payments.Infrastructure \
  --startup-project src/Payments/Payments.Api \
  -c PaymentsDbContext

# Add dependencia

dotnet add src/Payments/Payments.Api/Payments.Api.csproj reference src/Payments/Payments.Domain/Payments.Domain.csproj
dotnet add src/Payments/Payments.Application/Payments.Application.csproj reference src/Payments/Payments.Domain/Payments.Domain.csproj
dotnet add src/Payments/Payments.Infrastructure/Payments.Infrastructure.csproj reference src/Payments/Payments.Domain/Payments.Domain.csproj

# clean debug

- dotnet clean && dotnet build -c Debug
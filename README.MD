

# Criar Tópico Kafka

docker exec -it redpanda rpk topic create payments.requested payments.provider.chosen \
  payments.provider.callback payments.authorized payments.captured payments.failed \
  ledger.entries reconciliation.mismatch checkout.readmodel.updated

# Teste de conectividade 

- docker exec -it redpanda rpk topic list
- docker exec -it postgres sh
- psql -h localhost -U payments -d paymentsdb

# Executar migrations

- dotnet tool install --global dotnet-ef
- dotnet ef migrations add Init \
  --project src/Payments/Payments.Infrastructure \
  --startup-project src/Payments/Payments.Api \
  -c PaymentsDbContext
- dotnet ef database update \
  --project src/Payments/Payments.Infrastructure \
  --startup-project src/Payments/Payments.Api \
  -c PaymentsDbContext
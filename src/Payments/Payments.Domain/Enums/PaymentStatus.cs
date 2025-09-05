namespace Payments.Domain
{
    public enum PaymentStatus
    {
        Created,
        Processing,
        Completed,
        Failed,
        Cancelled,
    }
}

namespace Payments.Domain
{
    public enum PaymentStatus
    {
        Requested = 0,
        Authorized = 1,
        Captured = 2,
        Failed = 3,
        Refunded = 4,
    }
}

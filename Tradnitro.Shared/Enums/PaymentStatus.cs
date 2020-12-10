namespace Tradnitro.Shared.Enums
{
    public enum PaymentStatus
    {
        // The default state. A payment record is created for whatever reason. Should normally be attempted but will help distinguish between attempted payments and empty payment records
        Ready,

        // Payment has been attempted, yet to verify payment
        Attempted,

        // Payment has been verified, may be set by admin for cash payment and payment from other non-system-automated sources
        Succeeded,

        // Payment failed, the payment has failed
        Failed,

        // Payment made after invoice expiry, keep the status to identify such payments
        Late,

        // Payment made after invoice cancellation, keep the status to identify such payments
        Cancelled
    }
}
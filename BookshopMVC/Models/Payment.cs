using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopMVC.Models
{
    public enum PaymentStatus
    {
        Pending = 1,
        Succeeded = 2,
        Failed = 3
    }

    public enum PaymentMethod
    {
        CreditCard = 1,
        DebitCard = 2
    }

    public class Payment
    {
        public int Id { get; set; }

        // Link to order
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        // Payment amount
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // Payment method
        public PaymentMethod PaymentMethod { get; set; }

        // Payment status
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        // Stripe payment intent ID
        [MaxLength(100)]
        public string StripePaymentIntentId { get; set; } = string.Empty;

        // Stripe client secret for frontend
        [MaxLength(200)]
        public string StripeClientSecret { get; set; } = string.Empty;

        // Timestamps
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedDate { get; set; }

        // Helper properties
        public bool IsSuccessful => Status == PaymentStatus.Succeeded;
    }
}

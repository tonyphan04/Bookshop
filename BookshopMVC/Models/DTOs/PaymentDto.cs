using System.ComponentModel.DataAnnotations;
using BookshopMVC.Models;

namespace BookshopMVC.DTOs
{
    // Simple DTO for creating payments
    public class CreatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }
    }

    // DTO for payment status response
    public class PaymentStatusDto
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public bool IsCompleted => Status == PaymentStatus.Succeeded;
    }
}

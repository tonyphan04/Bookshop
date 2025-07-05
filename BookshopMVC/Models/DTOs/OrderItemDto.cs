using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Complete order item DTO with full book information and pricing details.
    /// Used in order detail views, admin order management, and order confirmations.
    /// Includes denormalized book information to avoid additional database queries.
    /// </summary>
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int BookId { get; set; }
        /// <summary>
        /// Book title for display without additional joins
        /// </summary>
        public string? BookTitle { get; set; }
        /// <summary>
        /// Book author for display without additional joins
        /// </summary>
        public string? BookAuthor { get; set; }
        /// <summary>
        /// Book image URL for display in order items
        /// </summary>
        public string? BookImageUrl { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// Price per unit at the time of order (historical pricing)
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Total price for this line item (Quantity × UnitPrice)
        /// </summary>
        public decimal TotalPrice { get; set; }
    }

    /// <summary>
    /// DTO for creating new order items with validation.
    /// Used during checkout process and order creation.
    /// Simple structure focusing on essential item selection data.
    /// </summary>
    public class CreateOrderItemDto
    {
        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// DTO for updating order item quantities with validation.
    /// Used in shopping cart updates and order modifications.
    /// Allows quantity changes while maintaining validation rules.
    /// </summary>
    public class UpdateOrderItemDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Simplified order item DTO for customer-facing displays.
    /// Used in order summaries, customer order history, and receipts.
    /// Excludes internal IDs and focuses on customer-relevant information.
    /// </summary>
    public class OrderItemSummaryDto
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookAuthor { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// Historical unit price at time of purchase
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// Total price for this line item (computed field)
        /// </summary>
        public decimal TotalPrice { get; set; }
    }

    /// <summary>
    /// Shopping cart item DTO with current pricing and stock information.
    /// Used for shopping cart functionality before order finalization.
    /// Includes real-time stock information for inventory validation.
    /// </summary>
    public class CartItemDto
    {
        public int BookId { get; set; }
        public string? BookTitle { get; set; }
        public string? BookAuthor { get; set; }
        public string? BookImageUrl { get; set; }
        /// <summary>
        /// Current book price (may differ from historical order prices)
        /// </summary>
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
        /// <summary>
        /// Total price for this cart item (BookPrice × Quantity)
        /// </summary>
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Current available stock for inventory validation
        /// </summary>
        public int AvailableStock { get; set; }
    }
}

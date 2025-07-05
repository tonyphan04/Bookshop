using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Complete order information DTO with customer details and all order items.
    /// Used for order detail views, admin order management, and order confirmations.
    /// Includes denormalized customer information and computed totals for UI convenience.
    /// </summary>
    public class OrderDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        /// <summary>
        /// Customer's full name for display without additional joins
        /// </summary>
        public string? CustomerName { get; set; }
        /// <summary>
        /// Customer's email for contact and receipt purposes
        /// </summary>
        public string? CustomerEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Total quantity of all items in the order (computed field)
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// Complete list of items in this order with book details
        /// </summary>
        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }

    /// <summary>
    /// DTO for creating new orders with validation.
    /// Used in checkout process and order placement API endpoints.
    /// Simple structure focusing on essential order creation data.
    /// </summary>
    public class CreateOrderDto
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }

    /// <summary>
    /// Lightweight order summary DTO for order lists and dashboards.
    /// Used in admin order management, customer order history lists, and reports.
    /// Excludes individual items for better performance in list views.
    /// </summary>
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string? CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Total quantity across all items (computed field)
        /// </summary>
        public int TotalItems { get; set; }
    }

    /// <summary>
    /// Order DTO specifically designed for customer order history displays.
    /// Used in customer-facing order history pages and account sections.
    /// Includes item summaries but excludes admin-specific information.
    /// </summary>
    public class CustomerOrderHistoryDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        /// <summary>
        /// Total quantity across all items (computed field)
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// Simplified item information for customer history view
        /// </summary>
        public List<OrderItemSummaryDto> Items { get; set; } = new List<OrderItemSummaryDto>();
    }
}

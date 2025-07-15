using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Complete order information DTO with user details and all order items.
    /// Used for order detail views, admin order management, and order confirmations.
    /// Includes denormalized user information and computed totals for UI convenience.
    /// </summary>
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        /// <summary>
        /// User's name for display without additional joins
        /// </summary>
        public string? UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Total count of all items in the order (computed field)
        /// </summary>
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// Order DTO with complete order items details.
    /// Used for order detail pages showing complete order information.
    /// </summary>
    public class OrderWithItemsDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Complete list of items in this order with book details
        /// </summary>
        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }

    /// <summary>
    /// DTO for creating new orders with validation.
    /// Used in checkout process and order placement API endpoints.
    /// Simple structure focusing on essential order creation data.
    /// </summary>
    public class CreateOrderDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }

    /// <summary>
    /// Lightweight order summary DTO for order lists and dashboards.
    /// Used in admin order management, user order history lists, and reports.
    /// Excludes individual items for better performance in list views.
    /// </summary>
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        /// <summary>
        /// Total quantity across all items (computed field)
        /// </summary>
        public int ItemCount { get; set; }
    }

    /// <summary>
    /// Order DTO specifically designed for user order history displays.
    /// Used in user-facing order history pages and account sections.
    /// Includes item summaries but excludes admin-specific information.
    /// </summary>
    public class UserOrderHistoryDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "Pending";
        /// <summary>
        /// Total quantity across all items (computed field)
        /// </summary>
        public int ItemCount { get; set; }
        /// <summary>
        /// Simplified item information for user history view
        /// </summary>
        public List<OrderItemSummaryDto> Items { get; set; } = new List<OrderItemSummaryDto>();
    }
}

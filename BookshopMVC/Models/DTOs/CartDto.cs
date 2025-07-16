using System.ComponentModel.DataAnnotations;
using BookshopMVC.Models;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Shopping cart item DTO for cart management and checkout.
    /// Essential for MVP - manages items before order placement.
    /// </summary>
    public class CartItemDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; } = string.Empty;
        public string? BookImageUrl { get; set; }
        public decimal BookPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime AddedDate { get; set; }

        // For stock validation
        public int AvailableStock { get; set; }
        public bool InStock => AvailableStock > 0;
    }

    /// <summary>
    /// DTO for adding items to cart.
    /// </summary>
    public class AddToCartDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;
    }

    /// <summary>
    /// DTO for updating cart item quantities.
    /// </summary>
    public class UpdateCartItemDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Complete cart summary for checkout page.
    /// </summary>
    public class CartSummaryDto
    {
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
        public bool HasItems => Items.Any();
    }

    /// <summary>
    /// Full cart DTO with all cart information.
    /// </summary>
    public class CartDto
    {
        public int UserId { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalItems => Items.Sum(i => i.Quantity);
        public decimal TotalPrice => Items.Sum(i => i.TotalPrice);
        public bool HasItems => Items.Any();
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// DTO for checkout process.
    /// </summary>
    public class CheckoutDto
    {
        [Required]
        public int UserId { get; set; }

        [Required, MaxLength(200)]
        public string ShippingAddress { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// DTO for updating order status.
    /// </summary>
    public class UpdateOrderStatusDto
    {
        [Required]
        public OrderStatus Status { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}

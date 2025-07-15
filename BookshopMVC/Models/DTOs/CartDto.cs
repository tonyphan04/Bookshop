using System.ComponentModel.DataAnnotations;

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
}

using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.Models
{
    /// <summary>
    /// Shopping cart to store items before checkout
    /// Essential for MVP - allows users to add books and manage quantities before ordering
    /// </summary>
    public class CartItem
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int BookId { get; set; }
        public Book Book { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        // Helper computed property
        public decimal TotalPrice => (Book?.Price ?? 0) * Quantity;
    }
}

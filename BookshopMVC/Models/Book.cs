using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookshopMVC.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 13-digit ISBN number
        /// </summary>
        [Required, MaxLength(13)]
        public string ISBN13 { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int Stock { get; set; } = 0;

        [MaxLength(300)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Publisher name as simple string (no separate table for MVP)
        /// </summary>
        [MaxLength(150)]
        public string? Publisher { get; set; }

        /// <summary>
        /// Language - defaulting to English for MVP
        /// </summary>
        [MaxLength(50)]
        public string Language { get; set; } = "English";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        public int GenreId { get; set; }

        // Navigation Properties
        public Genre Genre { get; set; } = null!;
        public ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}



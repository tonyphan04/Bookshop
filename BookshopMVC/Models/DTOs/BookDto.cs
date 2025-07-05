using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Complete book information DTO for detailed book displays and API responses.
    /// Used for book detail pages, edit forms, and comprehensive book information.
    /// Includes computed CategoryName to avoid additional database queries.
    /// </summary>
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; }
        public int CategoryId { get; set; }
        /// <summary>
        /// Denormalized category name for display without additional joins
        /// </summary>
        public string CategoryName { get; set; }
    }

    /// <summary>
    /// DTO for creating new books with validation attributes.
    /// Used in admin book creation forms and API endpoints.
    /// Excludes Id (auto-generated) and computed properties.
    /// Contains validation rules to ensure data integrity.
    /// </summary>
    public class CreateBookDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; }

        [Required, MaxLength(100)]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// DTO for updating existing books with validation attributes.
    /// Used in admin book edit forms and API endpoints.
    /// Similar to CreateBookDto but for updates to existing records.
    /// Maintains same validation rules as creation.
    /// </summary>
    public class UpdateBookDto
    {
        [Required, MaxLength(150)]
        public string Title { get; set; }

        [Required, MaxLength(100)]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }

    /// <summary>
    /// Lightweight book summary DTO for list displays and search results.
    /// Used in book catalogs, search results, category pages, and card displays.
    /// Excludes heavy fields like description and includes computed properties.
    /// Optimized for performance in list scenarios.
    /// </summary>
    public class BookSummaryDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Author { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? CategoryName { get; set; }
        /// <summary>
        /// Computed property indicating if the book is available for purchase
        /// </summary>
        public bool InStock { get; set; }
    }
}

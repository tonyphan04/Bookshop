using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Complete book information DTO for detailed book displays and API responses.
    /// Used for book detail pages, edit forms, and comprehensive book information.
    /// Includes computed GenreName and authors to avoid additional database queries.
    /// </summary>
    public class BookDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ISBN13 { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? ImageUrl { get; set; }
        public int GenreId { get; set; }
        public string Publisher { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// Denormalized genre name for display without additional joins
        /// </summary>
        public string? GenreName { get; set; }
        
        /// <summary>
        /// List of author names for this book
        /// </summary>
        public List<string> Authors { get; set; } = new();
    }

    /// <summary>
    /// DTO for creating new books with validation attributes.
    /// Used in admin book creation forms and API endpoints.
    /// Excludes Id (auto-generated) and computed properties.
    /// Contains validation rules to ensure data integrity.
    /// </summary>
    public class CreateBookDto
    {
        [Required, MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(13, MinimumLength = 13)]
        [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "ISBN13 must be exactly 13 digits")]
        public string ISBN13 { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int GenreId { get; set; }

        [Required, MaxLength(150)]
        public string Publisher { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        /// <summary>
        /// List of author IDs for this book
        /// </summary>
        public List<int> AuthorIds { get; set; } = new();
    }

    /// <summary>
    /// DTO for updating existing books with validation attributes.
    /// Used in admin book edit forms and API endpoints.
    /// Similar to CreateBookDto but for updates to existing records.
    /// Maintains same validation rules as creation.
    /// </summary>
    public class UpdateBookDto
    {
        [Required, MaxLength(250)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(13, MinimumLength = 13)]
        [RegularExpression(@"^[0-9]{13}$", ErrorMessage = "ISBN13 must be exactly 13 digits")]
        public string ISBN13 { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock cannot be negative")]
        public int Stock { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int GenreId { get; set; }

        [Required, MaxLength(150)]
        public string Publisher { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Language { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        /// <summary>
        /// List of author IDs for this book
        /// </summary>
        public List<int> AuthorIds { get; set; } = new();
    }

    /// <summary>
    /// Lightweight book summary DTO for list displays and search results.
    /// Used in book catalogs, search results, genre pages, and card displays.
    /// Excludes heavy fields and includes computed properties.
    /// Optimized for performance in list scenarios.
    /// </summary>
    public class BookSummaryDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public decimal Price { get; set; }
        public string? ImageUrl { get; set; }
        public string? GenreName { get; set; }
        
        /// <summary>
        /// Computed property indicating if the book is available for purchase
        /// </summary>
        public bool InStock { get; set; }
        
        /// <summary>
        /// List of author names for this book
        /// </summary>
        public List<string> Authors { get; set; } = new();
    }
}

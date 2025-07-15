using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Complete author information DTO for author profiles and book details.
    /// Used for author detail pages, book displays, and admin author management.
    /// </summary>
    public class AuthorDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Biography { get; set; }
        public DateTime CreatedDate { get; set; }
        
        // Statistics for admin/detailed views
        public int BookCount { get; set; }
        
        // Books by this author (for detailed author page)
        public List<BookSummaryDto> Books { get; set; } = new();
    }

    /// <summary>
    /// DTO for creating new authors with validation.
    /// Used in admin author creation forms and API endpoints.
    /// </summary>
    public class CreateAuthorDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Biography { get; set; }
    }

    /// <summary>
    /// DTO for updating existing authors.
    /// Used in admin author editing forms and API endpoints.
    /// </summary>
    public class UpdateAuthorDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Biography { get; set; }
    }

    /// <summary>
    /// Lightweight author summary DTO for lists and book displays.
    /// Used in book listings, author dropdowns, and search results.
    /// Minimal information for performance in list scenarios.
    /// </summary>
    public class AuthorSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int BookCount { get; set; }
    }

    /// <summary>
    /// Author DTO specifically for book-author relationships.
    /// Used when displaying authors in book details or managing book-author associations.
    /// </summary>
    public class BookAuthorDto
    {
        public int AuthorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int AuthorOrder { get; set; } // For multiple authors on one book
    }
}

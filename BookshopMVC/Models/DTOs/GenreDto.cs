using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Complete genre information DTO for genre management and book categorization.
    /// Used for genre detail pages, book category selection, and admin genre management.
    /// </summary>
    public class GenreDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        
        // Statistics for admin view
        public int BookCount { get; set; }
        
        // Books in this genre (for detailed genre page)
        public List<BookSummaryDto> Books { get; set; } = new();
    }

    /// <summary>
    /// DTO for creating new genres with validation.
    /// Used in admin genre creation forms and API endpoints.
    /// </summary>
    public class CreateGenreDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        [Range(1, int.MaxValue)]
        public int DisplayOrder { get; set; } = 1;
    }

    /// <summary>
    /// DTO for updating existing genres.
    /// Used in admin genre editing forms and API endpoints.
    /// </summary>
    public class UpdateGenreDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; }

        [Range(1, int.MaxValue)]
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// Lightweight genre summary DTO for dropdowns and book displays.
    /// Used in book creation/editing forms, book listings, and navigation.
    /// Minimal information for performance in list scenarios.
    /// </summary>
    public class GenreSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int DisplayOrder { get; set; }
        public int BookCount { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Basic category information DTO with computed book count.
    /// Used for category listings, dropdown selections, and admin category management.
    /// Includes BookCount for displaying statistics without loading all books.
    /// </summary>
    public class CategoryDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        /// <summary>
        /// Number of books in this category (computed field)
        /// </summary>
        public int BookCount { get; set; }
    }

    /// <summary>
    /// DTO for creating new categories with validation.
    /// Used in admin category creation forms and API endpoints.
    /// Simple structure focusing on essential category data.
    /// </summary>
    public class CreateCategoryDto
    {
        [Required, MaxLength(50)]
        public string? Name { get; set; }
    }

    /// <summary>
    /// DTO for updating existing categories with validation.
    /// Used in admin category edit forms and API endpoints.
    /// Maintains same validation rules as creation.
    /// </summary>
    public class UpdateCategoryDto
    {
        [Required, MaxLength(50)]
        public string? Name { get; set; }
    }

    /// <summary>
    /// Category DTO that includes all books within the category.
    /// Used for category detail pages and comprehensive category displays.
    /// WARNING: Can be memory-intensive for categories with many books.
    /// Should be used with careful consideration of the book collection size.
    /// </summary>
    public class CategoryWithBooksDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        /// <summary>
        /// Complete list of books in this category as summary DTOs
        /// </summary>
        public List<BookSummaryDto> Books { get; set; } = new List<BookSummaryDto>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string FullName => $"{FirstName} {LastName}";

        [MaxLength(500)]
        public string? Biography { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<AuthorBook> AuthorBooks { get; set; } = new List<AuthorBook>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace BookshopMVC.Models
{
    public class Genre
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public int DisplayOrder { get; set; } = 0;

        // Navigation properties
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Book
{
    public int Id { get; set; }

    [Required, MaxLength(150)]
    public string Title { get; set; }

    [Required, MaxLength(100)]
    public string Author { get; set; }

    [Required]
    public string ISBN { get; set; }

    [MaxLength(1000)]
    public string Description { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public int Stock { get; set; }

    public string ImageUrl { get; set; }

    // Relationships
    public int CategoryId { get; set; }
    public Category Category { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; }
}

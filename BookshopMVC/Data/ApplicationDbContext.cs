namespace BookshopMVC.Data;

using Microsoft.EntityFrameworkCore;
using BookshopMVC.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 📚 Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fiction" },
            new Category { Id = 2, Name = "Non-Fiction" },
            new Category { Id = 3, Name = "Technology" }
        );

        // 📘 Books
        modelBuilder.Entity<Book>().HasData(
            new Book
            {
                Id = 1,
                Title = "Clean Code",
                Author = "Robert C. Martin",
                ISBN = "9780132350884",
                Description = "A handbook of agile software craftsmanship",
                Price = 59.99M,
                Stock = 10,
                CategoryId = 3,
                ImageUrl = "/images/clean-code.jpg"
            },
            new Book
            {
                Id = 2,
                Title = "Atomic Habits",
                Author = "James Clear",
                ISBN = "9780735211292",
                Description = "Tiny changes, remarkable results",
                Price = 39.99M,
                Stock = 20,
                CategoryId = 2,
                ImageUrl = "/images/atomic-habits.jpg"
            }
        );

        // 👤 Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = 1,
                FullName = "Tony Phan",
                Email = "tony@example.com",
                PasswordHash = "hashed_password",
                Address = "123 Developer Lane"
            }
        );
    }

}



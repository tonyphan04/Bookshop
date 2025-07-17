using Microsoft.EntityFrameworkCore;
using BookshopMVC.Models;

namespace BookshopMVC.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<AuthorBook> AuthorBooks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            // AuthorBook many-to-many relationship
            modelBuilder.Entity<AuthorBook>()
                .HasKey(ab => new { ab.AuthorId, ab.BookId });

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Author)
                .WithMany(a => a.AuthorBooks)
                .HasForeignKey(ab => ab.AuthorId);

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Book)
                .WithMany(b => b.AuthorBooks)
                .HasForeignKey(ab => ab.BookId);

            // Book-Genre relationship
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Genre)
                .WithMany(g => g.Books)
                .HasForeignKey(b => b.GenreId);

            // Order-User relationship
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            // OrderItem relationships
            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Book)
                .WithMany(b => b.OrderItems)
                .HasForeignKey(oi => oi.BookId);

            // CartItem relationships
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId);

            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Book)
                .WithMany()
                .HasForeignKey(ci => ci.BookId);

            // Configure unique constraint for cart items (one book per user in cart)
            modelBuilder.Entity<CartItem>()
                .HasIndex(ci => new { ci.UserId, ci.BookId })
                .IsUnique();

            // Payment relationships
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId);

            // Configure precision for decimal properties
            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Order>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);

            // Configure TotalPrice as computed property (not stored in database)
            modelBuilder.Entity<OrderItem>()
                .Ignore(oi => oi.TotalPrice);

            // Seed data for MVP

            // 📚 Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre
                {
                    Id = 1,
                    Name = "Technology",
                    Description = "Books about programming, software development, and technology",
                    IsActive = true,
                    DisplayOrder = 1
                },
                new Genre
                {
                    Id = 2,
                    Name = "Self-Help",
                    Description = "Books about personal development and productivity",
                    IsActive = true,
                    DisplayOrder = 2
                },
                new Genre
                {
                    Id = 3,
                    Name = "Fiction",
                    Description = "Novels and fictional stories",
                    IsActive = true,
                    DisplayOrder = 3
                }
            );

            // 👤 Authors
            modelBuilder.Entity<Author>().HasData(
                new Author
                {
                    Id = 1,
                    FirstName = "Robert C.",
                    LastName = "Martin",
                    Biography = "Software engineer and author, known for Clean Code principles",
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Author
                {
                    Id = 2,
                    FirstName = "James",
                    LastName = "Clear",
                    Biography = "Author and speaker focused on habits, decision making, and continuous improvement",
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 📘 Books
            modelBuilder.Entity<Book>().HasData(
                new Book
                {
                    Id = 1,
                    Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    ISBN13 = "9780132350884",
                    Price = 59.99M,
                    Stock = 10,
                    GenreId = 1,
                    Publisher = "Prentice Hall",
                    Language = "English",
                    ImageUrl = "/images/clean-code.jpg",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Book
                {
                    Id = 2,
                    Title = "Atomic Habits: An Easy & Proven Way to Build Good Habits & Break Bad Ones",
                    ISBN13 = "9780735211292",
                    Price = 39.99M,
                    Stock = 20,
                    GenreId = 2,
                    Publisher = "Avery",
                    Language = "English",
                    ImageUrl = "/images/atomic-habits.jpg",
                    IsActive = true,
                    CreatedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // 📚 AuthorBook relationships
            modelBuilder.Entity<AuthorBook>().HasData(
                new AuthorBook { AuthorId = 1, BookId = 1, AuthorOrder = 1 },
                new AuthorBook { AuthorId = 2, BookId = 2, AuthorOrder = 1 }
            );

            // 👤 Users (sample admin and customer)
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@bookshop.com",
                    PasswordHash = "hashed_admin_password", // In real app, use proper password hashing
                    Phone = "+1-555-0001",
                    Address = "123 Admin Street, Admin City",
                    Role = UserRole.Admin,
                    IsActive = true,
                    RegistrationDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = 2,
                    FirstName = "Tony",
                    LastName = "Phan",
                    Email = "tony@example.com",
                    PasswordHash = "hashed_customer_password", // In real app, use proper password hashing
                    Phone = "+1-555-0002",
                    Address = "123 Developer Lane, Tech City",
                    Role = UserRole.Customer,
                    IsActive = true,
                    RegistrationDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }
    }
}

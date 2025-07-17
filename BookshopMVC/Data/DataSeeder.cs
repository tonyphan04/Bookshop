using Microsoft.EntityFrameworkCore;
using BookshopMVC.Models;
using System.Security.Cryptography;
using System.Text;

namespace BookshopMVC.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if data already exists
            if (await context.Users.AnyAsync() || await context.Books.AnyAsync())
            {
                return; // Database already seeded
            }

            // Helper method to hash passwords
            string HashPassword(string password)
            {
                using var sha256 = SHA256.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }

            // Seed Genres
            var genres = new List<Genre>
            {
                new Genre { Name = "Programming", Description = "Books about programming languages and software development", IsActive = true, DisplayOrder = 1 },
                new Genre { Name = "Science Fiction", Description = "Futuristic and imaginative fiction", IsActive = true, DisplayOrder = 2 },
                new Genre { Name = "Business", Description = "Books on business strategy, management, and entrepreneurship", IsActive = true, DisplayOrder = 3 },
                new Genre { Name = "Self Help", Description = "Personal development and motivational books", IsActive = true, DisplayOrder = 4 },
                new Genre { Name = "History", Description = "Historical events, biographies, and cultural studies", IsActive = true, DisplayOrder = 5 },
                new Genre { Name = "Fantasy", Description = "Magical and mythical fiction", IsActive = true, DisplayOrder = 6 },
                new Genre { Name = "Technology", Description = "Books about emerging technologies and innovation", IsActive = true, DisplayOrder = 7 },
                new Genre { Name = "Biography", Description = "Life stories of notable people", IsActive = true, DisplayOrder = 8 }
            };

            await context.Genres.AddRangeAsync(genres);
            await context.SaveChangesAsync();

            // Seed Authors
            var authors = new List<Author>
            {
                new Author { FirstName = "Robert", LastName = "Martin", Biography = "Also known as Uncle Bob, is an American software engineer and author. He is a co-author of the Agile Manifesto.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Martin", LastName = "Fowler", Biography = "British software developer, author and international public speaker on software development.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Eric", LastName = "Freeman", Biography = "Co-author of Head First Design Patterns and other programming books.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Kathy", LastName = "Sierra", Biography = "American programming instructor and game developer, co-author of the Head First series.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Steve", LastName = "McConnell", Biography = "Author of Code Complete and other software engineering books.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Andy", LastName = "Hunt", Biography = "Co-author of The Pragmatic Programmer and founding member of the Agile Alliance.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Dave", LastName = "Thomas", Biography = "Co-author of The Pragmatic Programmer and founder of The Pragmatic Bookshelf.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Gang", LastName = "of Four", Biography = "Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides - authors of Design Patterns.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Frank", LastName = "Herbert", Biography = "American science fiction writer best known for the Dune series.", CreatedDate = DateTime.UtcNow },
                new Author { FirstName = "Isaac", LastName = "Asimov", Biography = "American writer and professor of biochemistry, known for his works of science fiction.", CreatedDate = DateTime.UtcNow }
            };

            await context.Authors.AddRangeAsync(authors);
            await context.SaveChangesAsync();

            // Seed Users (Test accounts)
            var users = new List<User>
            {
                new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@bookshop.com",
                    PasswordHash = HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    IsActive = true,
                    Phone = "555-0001",
                    Address = "123 Admin Street, Admin City, AC 12345",
                    RegistrationDate = DateTime.UtcNow.AddDays(-30)
                },
                new User
                {
                    FirstName = "John",
                    LastName = "Customer",
                    Email = "customer@bookshop.com",
                    PasswordHash = HashPassword("Customer123!"),
                    Role = UserRole.Customer,
                    IsActive = true,
                    Phone = "555-0002",
                    Address = "456 Customer Lane, Customer City, CC 67890",
                    RegistrationDate = DateTime.UtcNow.AddDays(-20)
                },
                new User
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Email = "jane.smith@email.com",
                    PasswordHash = HashPassword("Password123!"),
                    Role = UserRole.Customer,
                    IsActive = true,
                    Phone = "555-0003",
                    Address = "789 Smith Avenue, Smith Town, ST 13579",
                    RegistrationDate = DateTime.UtcNow.AddDays(-15)
                },
                new User
                {
                    FirstName = "Test",
                    LastName = "User",
                    Email = "test@bookshop.com",
                    PasswordHash = HashPassword("Test123!"),
                    Role = UserRole.Customer,
                    IsActive = true,
                    Phone = "555-0004",
                    Address = "321 Test Street, Test City, TC 24680",
                    RegistrationDate = DateTime.UtcNow.AddDays(-10)
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();

            // Seed Books
            var books = new List<Book>
            {
                new Book
                {
                    Title = "Clean Code: A Handbook of Agile Software Craftsmanship",
                    Description = "Even bad code can function. But if code isn't clean, it can bring a development organization to its knees.",
                    Price = 42.99m,
                    Stock = 25,
                    GenreId = 1,  // Programming
                    ImageUrl = "https://example.com/clean-code.jpg",
                    ISBN13 = "9780132350884",
                    Publisher = "Prentice Hall",
                    CreatedDate = DateTime.UtcNow.AddDays(-60)
                },
                new Book
                {
                    Title = "Refactoring: Improving the Design of Existing Code",
                    Description = "Refactoring is a controlled technique for improving the design of an existing code base.",
                    Price = 54.99m,
                    Stock = 18,
                    GenreId = 1,  // Programming
                    ImageUrl = "https://example.com/refactoring.jpg",
                    ISBN13 = "9780134757599",
                    Publisher = "Addison-Wesley",
                    CreatedDate = DateTime.UtcNow.AddDays(-55)
                },
                new Book
                {
                    Title = "Head First Design Patterns",
                    Description = "You'll learn design patterns in a way that won't put you to sleep! We think your time is too valuable to spend struggling with new concepts.",
                    Price = 49.99m,
                    Stock = 30,
                    GenreId = 1,  // Programming
                    ImageUrl = "https://example.com/head-first-patterns.jpg",
                    ISBN13 = "9780596007126",
                    Publisher = "O'Reilly Media",
                    CreatedDate = DateTime.UtcNow.AddDays(-50)
                },
                new Book
                {
                    Title = "Code Complete: A Practical Handbook of Software Construction",
                    Description = "Widely considered one of the best practical guides to programming, Steve McConnell's original CODE COMPLETE has been helping developers write better software for more than a decade.",
                    Price = 48.99m,
                    Stock = 22,
                    GenreId = 1,  // Programming
                    ImageUrl = "https://example.com/code-complete.jpg",
                    ISBN13 = "9780735619678",
                    Publisher = "Microsoft Press",
                    CreatedDate = DateTime.UtcNow.AddDays(-45)
                },
                new Book
                {
                    Title = "The Pragmatic Programmer: Your Journey To Mastery",
                    Description = "The Pragmatic Programmer is one of those rare tech books you'll read, re-read, and read again over the years.",
                    Price = 44.99m,
                    Stock = 35,
                    GenreId = 1,  // Programming
                    ImageUrl = "https://example.com/pragmatic-programmer.jpg",
                    ISBN13 = "9780135957059",
                    Publisher = "Addison-Wesley",
                    CreatedDate = DateTime.UtcNow.AddDays(-40)
                },
                new Book
                {
                    Title = "Dune",
                    Description = "Set on the desert planet Arrakis, Dune is the story of the boy Paul Atreides, heir to a noble family tasked with ruling an inhospitable world.",
                    Price = 16.99m,
                    Stock = 45,
                    GenreId = 2,  // Science Fiction
                    ImageUrl = "https://example.com/dune.jpg",
                    ISBN13 = "9780441013593",
                    Publisher = "Ace Books",
                    CreatedDate = DateTime.UtcNow.AddDays(-35)
                },
                new Book
                {
                    Title = "Foundation",
                    Description = "For twelve thousand years the Galactic Empire has ruled supreme. Now it is dying.",
                    Price = 15.99m,
                    Stock = 28,
                    GenreId = 2,   // Science Fiction
                    ImageUrl = "https://example.com/foundation.jpg",
                    ISBN13 = "9780553293357",
                    Publisher = "Bantam Spectra",
                    CreatedDate = DateTime.UtcNow.AddDays(-30)
                },
                new Book
                {
                    Title = "Design Patterns: Elements of Reusable Object-Oriented Software",
                    Description = "Capturing a wealth of experience about the design of object-oriented software, four top-notch designers present a catalog of simple and succinct solutions to commonly occurring design problems.",
                    Price = 59.99m,
                    Stock = 15,
                    GenreId = 1,  // Programming
                    ImageUrl = "https://example.com/design-patterns.jpg",
                    ISBN13 = "9780201633610",
                    Publisher = "Addison-Wesley",
                    CreatedDate = DateTime.UtcNow.AddDays(-25)
                },
                new Book
                {
                    Title = "Head First Java",
                    Description = "Head First Java is a complete learning experience for Java and object-oriented programming.",
                    Price = 47.99m,
                    Stock = 20,
                    GenreId = 1,  // Programming
                    ImageUrl = "https://example.com/head-first-java.jpg",
                    ISBN13 = "9780596009205",
                    Publisher = "O'Reilly Media",
                    CreatedDate = DateTime.UtcNow.AddDays(-20)
                },
                new Book
                {
                    Title = "Artificial Intelligence: A Modern Approach",
                    Description = "The most comprehensive, up-to-date introduction to the theory and practice of artificial intelligence.",
                    Price = 89.99m,
                    Stock = 12,
                    GenreId = 7,  // Technology
                    ImageUrl = "https://example.com/ai-modern-approach.jpg",
                    ISBN13 = "9780134610993",
                    Publisher = "Pearson",
                    CreatedDate = DateTime.UtcNow.AddDays(-15)
                }
            };

            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();

            // Seed Author-Book relationships (many-to-many)
            var authorBooks = new List<AuthorBook>
            {
                new AuthorBook { AuthorId = 1, BookId = 1 }, // Robert Martin - Clean Code
                new AuthorBook { AuthorId = 2, BookId = 2 }, // Martin Fowler - Refactoring
                new AuthorBook { AuthorId = 3, BookId = 3 }, // Eric Freeman - Head First Design Patterns
                new AuthorBook { AuthorId = 4, BookId = 3 }, // Kathy Sierra - Head First Design Patterns
                new AuthorBook { AuthorId = 5, BookId = 4 }, // Steve McConnell - Code Complete
                new AuthorBook { AuthorId = 6, BookId = 5 }, // Andy Hunt - Pragmatic Programmer
                new AuthorBook { AuthorId = 7, BookId = 5 }, // Dave Thomas - Pragmatic Programmer
                new AuthorBook { AuthorId = 9, BookId = 6 }, // Frank Herbert - Dune
                new AuthorBook { AuthorId = 10, BookId = 7 }, // Isaac Asimov - Foundation
                new AuthorBook { AuthorId = 8, BookId = 8 }, // Gang of Four - Design Patterns
                new AuthorBook { AuthorId = 4, BookId = 9 }, // Kathy Sierra - Head First Java
                new AuthorBook { AuthorId = 3, BookId = 9 }, // Eric Freeman - Head First Java
                new AuthorBook { AuthorId = 7, BookId = 10 }  // Dave Thomas - AI Modern Approach (placeholder)
            };

            await context.AuthorBooks.AddRangeAsync(authorBooks);
            await context.SaveChangesAsync();

            // Seed some sample cart items for test users
            var cartItems = new List<CartItem>
            {
                new CartItem
                {
                    UserId = 2, // John Customer
                    BookId = 1, // Clean Code
                    Quantity = 1,
                    AddedDate = DateTime.UtcNow.AddDays(-2)
                },
                new CartItem
                {
                    UserId = 2, // John Customer
                    BookId = 6, // Dune
                    Quantity = 2,
                    AddedDate = DateTime.UtcNow.AddDays(-1)
                },
                new CartItem
                {
                    UserId = 3, // Jane Smith
                    BookId = 3, // Head First Design Patterns
                    Quantity = 1,
                    AddedDate = DateTime.UtcNow.AddHours(-12)
                }
            };

            await context.CartItems.AddRangeAsync(cartItems);
            await context.SaveChangesAsync();

            // Seed sample orders
            var orders = new List<Order>
            {
                new Order
                {
                    UserId = 3, // Jane Smith
                    OrderDate = DateTime.UtcNow.AddDays(-5),
                    TotalPrice = 64.98m, // 2 books total
                    Status = OrderStatus.Completed,
                    CreatedDate = DateTime.UtcNow.AddDays(-5)
                },
                new Order
                {
                    UserId = 4, // Test User
                    OrderDate = DateTime.UtcNow.AddDays(-2),
                    TotalPrice = 42.99m, // 1 book
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow.AddDays(-2)
                }
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            // Seed order items
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    OrderId = 1,
                    BookId = 2, // Refactoring
                    UnitPrice = 54.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = 1,
                    BookId = 7, // Foundation
                    UnitPrice = 15.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = 2,
                    BookId = 1, // Clean Code
                    UnitPrice = 42.99m,
                    Quantity = 1
                }
            };

            await context.OrderItems.AddRangeAsync(orderItems);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Database seeded successfully with test data!");
            Console.WriteLine("📧 Test Accounts Created:");
            Console.WriteLine("   Admin: admin@bookshop.com / Admin123!");
            Console.WriteLine("   Customer: customer@bookshop.com / Customer123!");
            Console.WriteLine("   Jane: jane.smith@email.com / Password123!");
            Console.WriteLine("   Test: test@bookshop.com / Test123!");
            Console.WriteLine($"📚 {books.Count} books added");
            Console.WriteLine($"👥 {authors.Count} authors added");
            Console.WriteLine($"🏷️ {genres.Count} genres added");
            Console.WriteLine($"🛒 {cartItems.Count} cart items added");
            Console.WriteLine($"📦 {orders.Count} orders added");
        }
    }
}

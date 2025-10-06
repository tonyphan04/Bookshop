using Microsoft.EntityFrameworkCore;
using BookshopMVC.Models;
using Microsoft.AspNetCore.Identity;

namespace BookshopMVC.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, IPasswordHasher<User> passwordHasher)
        {
            // Apply pending migrations
            await context.Database.MigrateAsync();

            // Check if data already exists
            if (await context.Users.AnyAsync() || await context.Books.AnyAsync())
            {
                return; // Database already seeded
            }

            // Use Identity password hasher for secure seeded passwords

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

            // Seed Users (Test accounts with diverse scenarios)
            var users = new List<User>
            {
                new User
                {
                    FirstName = "Admin",
                    LastName = "User",
                    Email = "admin@bookshop.com",
                    PasswordHash = passwordHasher.HashPassword(null!, "Admin123!"),
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
                    PasswordHash = passwordHasher.HashPassword(null!, "Customer123!"),
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
                    PasswordHash = passwordHasher.HashPassword(null!, "Password123!"),
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
                    PasswordHash = passwordHasher.HashPassword(null!, "Test123!"),
                    Role = UserRole.Customer,
                    IsActive = true,
                    Phone = "555-0004",
                    Address = "321 Test Street, Test City, TC 24680",
                    RegistrationDate = DateTime.UtcNow.AddDays(-10)
                },
                new User
                {
                    FirstName = "Alice",
                    LastName = "Johnson",
                    Email = "alice.johnson@email.com",
                    PasswordHash = passwordHasher.HashPassword(null!, "Alice123!"),
                    Role = UserRole.Customer,
                    IsActive = true,
                    Phone = "555-0005",
                    Address = "555 Johnson Boulevard, Johnson City, JC 55555",
                    RegistrationDate = DateTime.UtcNow.AddDays(-8)
                },
                new User
                {
                    FirstName = "Bob",
                    LastName = "Wilson",
                    Email = "bob.wilson@email.com",
                    PasswordHash = passwordHasher.HashPassword(null!, "Bob123!"),
                    Role = UserRole.Customer,
                    IsActive = true,
                    Phone = "555-0006",
                    Address = "777 Wilson Drive, Wilson Town, WT 77777",
                    RegistrationDate = DateTime.UtcNow.AddDays(-5)
                },
                new User
                {
                    FirstName = "Charlie",
                    LastName = "Brown",
                    Email = "charlie.brown@email.com",
                    PasswordHash = passwordHasher.HashPassword(null!, "Charlie123!"),
                    Role = UserRole.Customer,
                    IsActive = true,
                    Phone = "555-0007",
                    Address = "888 Brown Street, Brown City, BC 88888",
                    RegistrationDate = DateTime.UtcNow.AddDays(-3)
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
                },
                new Book
                {
                    Title = "The Art of War",
                    Description = "An ancient Chinese military treatise dating from the 5th century BC.",
                    Price = 12.99m,
                    Stock = 40,
                    GenreId = 5,  // History
                    ImageUrl = "https://example.com/art-of-war.jpg",
                    ISBN13 = "9780486425573",
                    Publisher = "Dover Publications",
                    CreatedDate = DateTime.UtcNow.AddDays(-10)
                },
                new Book
                {
                    Title = "Think and Grow Rich",
                    Description = "Napoleon Hill's classic guide to success and wealth building.",
                    Price = 14.99m,
                    Stock = 35,
                    GenreId = 4,  // Self Help
                    ImageUrl = "https://example.com/think-grow-rich.jpg",
                    ISBN13 = "9781585424331",
                    Publisher = "Jeremy P. Tarcher",
                    CreatedDate = DateTime.UtcNow.AddDays(-8)
                },
                new Book
                {
                    Title = "The Lean Startup",
                    Description = "How Today's Entrepreneurs Use Continuous Innovation to Create Radically Successful Businesses.",
                    Price = 24.99m,
                    Stock = 28,
                    GenreId = 3,  // Business
                    ImageUrl = "https://example.com/lean-startup.jpg",
                    ISBN13 = "9780307887894",
                    Publisher = "Crown Business",
                    CreatedDate = DateTime.UtcNow.AddDays(-5)
                },
                new Book
                {
                    Title = "The Lord of the Rings",
                    Description = "J.R.R. Tolkien's epic fantasy trilogy in one volume.",
                    Price = 32.99m,
                    Stock = 25,
                    GenreId = 6,  // Fantasy
                    ImageUrl = "https://example.com/lotr.jpg",
                    ISBN13 = "9780544003415",
                    Publisher = "Houghton Mifflin Harcourt",
                    CreatedDate = DateTime.UtcNow.AddDays(-3)
                },
                new Book
                {
                    Title = "Steve Jobs",
                    Description = "The exclusive biography of Steve Jobs by Walter Isaacson.",
                    Price = 18.99m,
                    Stock = 30,
                    GenreId = 8,  // Biography
                    ImageUrl = "https://example.com/steve-jobs.jpg",
                    ISBN13 = "9781451648539",
                    Publisher = "Simon & Schuster",
                    CreatedDate = DateTime.UtcNow.AddDays(-2)
                },
                new Book
                {
                    Title = "Clean Architecture",
                    Description = "A Craftsman's Guide to Software Structure and Design.",
                    Price = 36.99m,
                    Stock = 20,
                    GenreId = 1,  // Programming
                    ImageUrl = "https://example.com/clean-architecture.jpg",
                    ISBN13 = "9780134494166",
                    Publisher = "Prentice Hall",
                    CreatedDate = DateTime.UtcNow.AddDays(-1)
                },
                new Book
                {
                    Title = "The Hitchhiker's Guide to the Galaxy",
                    Description = "A comedy science fiction franchise created by Douglas Adams.",
                    Price = 13.99m,
                    Stock = 45,
                    GenreId = 2,  // Science Fiction
                    ImageUrl = "https://example.com/hitchhikers-guide.jpg",
                    ISBN13 = "9780345391803",
                    Publisher = "Del Rey Books",
                    CreatedDate = DateTime.UtcNow
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
                new AuthorBook { AuthorId = 7, BookId = 10 }, // Dave Thomas - AI Modern Approach (placeholder)
                new AuthorBook { AuthorId = 9, BookId = 11 }, // Frank Herbert - The Art of War (placeholder)
                new AuthorBook { AuthorId = 10, BookId = 12 }, // Isaac Asimov - Think and Grow Rich (placeholder)
                new AuthorBook { AuthorId = 5, BookId = 13 }, // Steve McConnell - The Lean Startup (placeholder)
                new AuthorBook { AuthorId = 6, BookId = 14 }, // Andy Hunt - The Lord of the Rings (placeholder)
                new AuthorBook { AuthorId = 7, BookId = 15 }, // Dave Thomas - Steve Jobs (placeholder)
                new AuthorBook { AuthorId = 1, BookId = 16 }, // Robert Martin - Clean Architecture
                new AuthorBook { AuthorId = 2, BookId = 17 } // Martin Fowler - Hitchhiker's Guide (placeholder)
            };

            await context.AuthorBooks.AddRangeAsync(authorBooks);
            await context.SaveChangesAsync();

            // Seed more comprehensive cart items for different users
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
                },
                new CartItem
                {
                    UserId = 5, // Alice Johnson
                    BookId = 11, // The Art of War
                    Quantity = 1,
                    AddedDate = DateTime.UtcNow.AddHours(-6)
                },
                new CartItem
                {
                    UserId = 5, // Alice Johnson
                    BookId = 12, // Think and Grow Rich
                    Quantity = 2,
                    AddedDate = DateTime.UtcNow.AddHours(-4)
                },
                new CartItem
                {
                    UserId = 6, // Bob Wilson
                    BookId = 14, // The Lord of the Rings
                    Quantity = 1,
                    AddedDate = DateTime.UtcNow.AddHours(-2)
                }
            };

            await context.CartItems.AddRangeAsync(cartItems);
            await context.SaveChangesAsync();

            // Seed diverse orders for payment testing - different statuses and amounts
            var orders = new List<Order>
            {
                // Completed order (already paid)
                new Order
                {
                    UserId = 3, // Jane Smith
                    OrderDate = DateTime.UtcNow.AddDays(-5),
                    TotalPrice = 70.98m, // 2 books total
                    Status = OrderStatus.Completed,
                    CreatedDate = DateTime.UtcNow.AddDays(-5)
                },
                // Pending order (ready for payment testing)
                new Order
                {
                    UserId = 4, // Test User
                    OrderDate = DateTime.UtcNow.AddDays(-2),
                    TotalPrice = 42.99m, // 1 book
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow.AddDays(-2)
                },
                // Another pending order (higher amount)
                new Order
                {
                    UserId = 2, // John Customer
                    OrderDate = DateTime.UtcNow.AddDays(-1),
                    TotalPrice = 127.97m, // 3 books
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow.AddDays(-1)
                },
                // Small order (low amount for testing)
                new Order
                {
                    UserId = 5, // Alice Johnson
                    OrderDate = DateTime.UtcNow.AddHours(-12),
                    TotalPrice = 12.99m, // 1 book
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow.AddHours(-12)
                },
                // Large order (high amount for testing)
                new Order
                {
                    UserId = 6, // Bob Wilson
                    OrderDate = DateTime.UtcNow.AddHours(-6),
                    TotalPrice = 234.95m, // 5 books
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow.AddHours(-6)
                },
                // Confirmed order (payment successful)
                new Order
                {
                    UserId = 7, // Charlie Brown
                    OrderDate = DateTime.UtcNow.AddHours(-3),
                    TotalPrice = 49.98m, // 2 books
                    Status = OrderStatus.Confirmed,
                    CreatedDate = DateTime.UtcNow.AddHours(-3)
                },
                // Cancelled order (payment failed/cancelled)
                new Order
                {
                    UserId = 3, // Jane Smith
                    OrderDate = DateTime.UtcNow.AddHours(-1),
                    TotalPrice = 89.99m, // 1 expensive book
                    Status = OrderStatus.Cancelled,
                    CreatedDate = DateTime.UtcNow.AddHours(-1)
                }
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            // Seed order items for all orders
            var orderItems = new List<OrderItem>
            {
                // Order 1 items (Jane Smith - Completed)
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
                
                // Order 2 items (Test User - Pending)
                new OrderItem
                {
                    OrderId = 2,
                    BookId = 1, // Clean Code
                    UnitPrice = 42.99m,
                    Quantity = 1
                },
                
                // Order 3 items (John Customer - Pending)
                new OrderItem
                {
                    OrderId = 3,
                    BookId = 3, // Head First Design Patterns
                    UnitPrice = 49.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = 3,
                    BookId = 5, // Pragmatic Programmer
                    UnitPrice = 44.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = 3,
                    BookId = 6, // Dune
                    UnitPrice = 16.99m,
                    Quantity = 2
                },
                
                // Order 4 items (Alice Johnson - Pending)
                new OrderItem
                {
                    OrderId = 4,
                    BookId = 11, // The Art of War
                    UnitPrice = 12.99m,
                    Quantity = 1
                },
                
                // Order 5 items (Bob Wilson - Pending)
                new OrderItem
                {
                    OrderId = 5,
                    BookId = 10, // AI Modern Approach
                    UnitPrice = 89.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = 5,
                    BookId = 13, // The Lean Startup
                    UnitPrice = 24.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = 5,
                    BookId = 14, // The Lord of the Rings
                    UnitPrice = 32.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = 5,
                    BookId = 15, // Steve Jobs
                    UnitPrice = 18.99m,
                    Quantity = 2
                },
                new OrderItem
                {
                    OrderId = 5,
                    BookId = 17, // Hitchhiker's Guide
                    UnitPrice = 13.99m,
                    Quantity = 5
                },
                
                // Order 6 items (Charlie Brown - Confirmed)
                new OrderItem
                {
                    OrderId = 6,
                    BookId = 12, // Think and Grow Rich
                    UnitPrice = 14.99m,
                    Quantity = 1
                },
                new OrderItem
                {
                    OrderId = 6,
                    BookId = 16, // Clean Architecture
                    UnitPrice = 36.99m,
                    Quantity = 1
                },
                
                // Order 7 items (Jane Smith - Processing)
                new OrderItem
                {
                    OrderId = 7,
                    BookId = 10, // AI Modern Approach
                    UnitPrice = 89.99m,
                    Quantity = 1
                }
            };

            await context.OrderItems.AddRangeAsync(orderItems);
            await context.SaveChangesAsync();

            Console.WriteLine("✅ Database seeded successfully with comprehensive test data!");
            Console.WriteLine("\n📧 Test Accounts Created:");
            Console.WriteLine("   👨‍💼 Admin: admin@bookshop.com / Admin123!");
            Console.WriteLine("   👤 Customer: customer@bookshop.com / Customer123!");
            Console.WriteLine("   👩 Jane Smith: jane.smith@email.com / Password123!");
            Console.WriteLine("   🧪 Test User: test@bookshop.com / Test123!");
            Console.WriteLine("   👩‍💻 Alice Johnson: alice.johnson@email.com / Alice123!");
            Console.WriteLine("   👨‍💻 Bob Wilson: bob.wilson@email.com / Bob123!");
            Console.WriteLine("   👨‍🎓 Charlie Brown: charlie.brown@email.com / Charlie123!");

            Console.WriteLine($"\n� Data Summary:");
            Console.WriteLine($"   �📚 {books.Count} books added across {genres.Count} genres");
            Console.WriteLine($"   👥 {authors.Count} authors added");
            Console.WriteLine($"   👨‍👩‍👧‍👦 {users.Count} users created");
            Console.WriteLine($"   🛒 {cartItems.Count} cart items added");
            Console.WriteLine($"   📦 {orders.Count} orders created");
            Console.WriteLine($"   📋 {orderItems.Count} order items added");

            Console.WriteLine($"\n💳 Payment Test Cases Available:");
            Console.WriteLine($"   ✅ Order #1: Completed ($70.98) - Jane Smith");
            Console.WriteLine($"   ⏳ Order #2: Pending ($42.99) - Test User");
            Console.WriteLine($"   ⏳ Order #3: Pending ($127.97) - John Customer");
            Console.WriteLine($"   ⏳ Order #4: Pending ($12.99) - Alice Johnson");
            Console.WriteLine($"   ⏳ Order #5: Pending ($234.95) - Bob Wilson");
            Console.WriteLine($"   ✅ Order #6: Confirmed ($49.98) - Charlie Brown");
            Console.WriteLine($"   🔄 Order #7: Cancelled ($89.99) - Jane Smith");

            Console.WriteLine($"\n🧪 Payment Testing URLs:");
            Console.WriteLine($"   POST /api/Payment/create/2 (Test User - $42.99)");
            Console.WriteLine($"   POST /api/Payment/create/3 (John Customer - $127.97)");
            Console.WriteLine($"   POST /api/Payment/create/4 (Alice Johnson - $12.99)");
            Console.WriteLine($"   POST /api/Payment/create/5 (Bob Wilson - $234.95)");
            Console.WriteLine($"   GET /api/Payment/status/{{orderId}} for any order");
            Console.WriteLine($"   POST /api/Payment/complete/{{orderId}} after payment");
        }
    }
}

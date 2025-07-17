using Microsoft.EntityFrameworkCore.Migrations;
using System.Security.Cryptography;
using System.Text;

#nullable disable

namespace BookshopMVC.Migrations
{
    /// <inheritdoc />
    public partial class SeedTestData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Helper method to hash passwords
            string HashPassword(string password)
            {
                using var sha256 = SHA256.Create();
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }

            // Insert Genres
            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Name", "Description", "IsActive", "DisplayOrder" },
                values: new object[,]
                {
                    { "Programming", "Books about programming languages and software development", true, 1 },
                    { "Science Fiction", "Futuristic and imaginative fiction", true, 2 },
                    { "Business", "Books on business strategy, management, and entrepreneurship", true, 3 },
                    { "Self Help", "Personal development and motivational books", true, 4 },
                    { "History", "Historical events, biographies, and cultural studies", true, 5 },
                    { "Fantasy", "Magical and mythical fiction", true, 6 },
                    { "Technology", "Books about emerging technologies and innovation", true, 7 },
                    { "Biography", "Life stories of notable people", true, 8 }
                });

            // Insert Authors
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "FirstName", "LastName", "Biography", "CreatedDate" },
                values: new object[,]
                {
                    { "Robert", "Martin", "Also known as Uncle Bob, is an American software engineer and author. He is a co-author of the Agile Manifesto.", DateTime.UtcNow },
                    { "Martin", "Fowler", "British software developer, author and international public speaker on software development.", DateTime.UtcNow },
                    { "Eric", "Freeman", "Co-author of Head First Design Patterns and other programming books.", DateTime.UtcNow },
                    { "Kathy", "Sierra", "American programming instructor and game developer, co-author of the Head First series.", DateTime.UtcNow },
                    { "Steve", "McConnell", "Author of Code Complete and other software engineering books.", DateTime.UtcNow },
                    { "Andy", "Hunt", "Co-author of The Pragmatic Programmer and founding member of the Agile Alliance.", DateTime.UtcNow },
                    { "Dave", "Thomas", "Co-author of The Pragmatic Programmer and programming language creator.", DateTime.UtcNow },
                    { "Joshua", "Bloch", "American software engineer who led the design and implementation of numerous Java platform features.", DateTime.UtcNow },
                    { "Gayle", "McDowell", "Software engineer and author, founder of CareerCup.", DateTime.UtcNow },
                    { "Jon", "Skeet", "British software engineer and author, known for his expertise in C# and .NET.", DateTime.UtcNow }
                });

            // Insert Users (Test accounts)
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "FirstName", "LastName", "Email", "PasswordHash", "Role", "CreatedDate" },
                values: new object[,]
                {
                    { "Admin", "User", "admin@bookshop.com", HashPassword("Admin123!"), "Admin", DateTime.UtcNow },
                    { "John", "Customer", "customer@bookshop.com", HashPassword("Customer123!"), "Customer", DateTime.UtcNow },
                    { "Jane", "Smith", "jane@bookshop.com", HashPassword("Jane123!"), "Customer", DateTime.UtcNow },
                    { "Bob", "Johnson", "bob@bookshop.com", HashPassword("Bob123!"), "Customer", DateTime.UtcNow },
                    { "Alice", "Brown", "alice@bookshop.com", HashPassword("Alice123!"), "Customer", DateTime.UtcNow }
                });

            // Insert Books
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Title", "Description", "Price", "Stock", "AuthorId", "GenreId", "ImageUrl", "CreatedDate" },
                values: new object[,]
                {
                    // Programming Books
                    { "Clean Code", "A Handbook of Agile Software Craftsmanship", 42.99m, 25, 1, 1, "https://images-na.ssl-images-amazon.com/images/I/41SH-SvWPxL._SX376_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "Refactoring", "Improving the Design of Existing Code", 45.99m, 20, 2, 1, "https://images-na.ssl-images-amazon.com/images/I/41jEbK-jG+L._SX379_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "Head First Design Patterns", "A Brain-Friendly Guide", 39.99m, 30, 3, 1, "https://images-na.ssl-images-amazon.com/images/I/51rmlxN57sL._SX430_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "Code Complete", "A Practical Handbook of Software Construction", 49.99m, 15, 5, 1, "https://images-na.ssl-images-amazon.com/images/I/515iO%2B-PRUL._SX408_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "The Pragmatic Programmer", "Your Journey to Mastery", 44.99m, 18, 6, 1, "https://images-na.ssl-images-amazon.com/images/I/41as%2B%2BaUjnL._SX396_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "Effective Java", "Best Practices for the Java Platform", 38.99m, 22, 8, 1, "https://images-na.ssl-images-amazon.com/images/I/41jEbK-jG+L._SX379_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "Cracking the Coding Interview", "189 Programming Questions and Solutions", 35.99m, 40, 9, 1, "https://images-na.ssl-images-amazon.com/images/I/41oYsXjLvZL._SX348_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "C# in Depth", "What you need to master C#", 41.99m, 12, 10, 1, "https://images-na.ssl-images-amazon.com/images/I/51P6ZjlMLJL._SX397_BO1,204,203,200_.jpg", DateTime.UtcNow },

                    // Technology Books
                    { "The Innovator's Dilemma", "When New Technologies Cause Great Firms to Fail", 29.99m, 20, 2, 7, "https://images-na.ssl-images-amazon.com/images/I/41B4%2BoYJaEL._SX331_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "The Lean Startup", "How Today's Entrepreneurs Use Innovation", 24.99m, 35, 3, 3, "https://images-na.ssl-images-amazon.com/images/I/41T%2BZrZDGUL._SX329_BO1,204,203,200_.jpg", DateTime.UtcNow },

                    // Business Books
                    { "Good to Great", "Why Some Companies Make the Leap... and Others Don't", 27.99m, 25, 1, 3, "https://images-na.ssl-images-amazon.com/images/I/41dWozwGGGL._SX327_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "The 7 Habits of Highly Effective People", "Powerful Lessons in Personal Change", 19.99m, 50, 4, 4, "https://images-na.ssl-images-amazon.com/images/I/51S2DXSRM5L._SX328_BO1,204,203,200_.jpg", DateTime.UtcNow },

                    // Fiction Books for variety
                    { "Dune", "The epic science fiction masterpiece", 16.99m, 30, 5, 2, "https://images-na.ssl-images-amazon.com/images/I/41g5QpSq%2BVL._SX322_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "The Lord of the Rings", "The classic fantasy trilogy", 34.99m, 15, 6, 6, "https://images-na.ssl-images-amazon.com/images/I/51EstVXM1UL._SX331_BO1,204,203,200_.jpg", DateTime.UtcNow },
                    { "1984", "George Orwell's dystopian masterpiece", 14.99m, 40, 7, 2, "https://images-na.ssl-images-amazon.com/images/I/41D7aWzSVyL._SX276_BO1,204,203,200_.jpg", DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seed data in reverse order due to foreign key constraints
            migrationBuilder.DeleteData(table: "Books", keyColumn: "Title", keyValues: new object[]
            {
                "Clean Code", "Refactoring", "Head First Design Patterns", "Code Complete",
                "The Pragmatic Programmer", "Effective Java", "Cracking the Coding Interview",
                "C# in Depth", "The Innovator's Dilemma", "The Lean Startup", "Good to Great",
                "The 7 Habits of Highly Effective People", "Dune", "The Lord of the Rings", "1984"
            });

            migrationBuilder.DeleteData(table: "Users", keyColumn: "Email", keyValues: new object[]
            {
                "admin@bookshop.com", "customer@bookshop.com", "jane@bookshop.com",
                "bob@bookshop.com", "alice@bookshop.com"
            });

            migrationBuilder.DeleteData(table: "Authors", keyColumn: "LastName", keyValues: new object[]
            {
                "Martin", "Fowler", "Freeman", "Sierra", "McConnell",
                "Hunt", "Thomas", "Bloch", "McDowell", "Skeet"
            });

            migrationBuilder.DeleteData(table: "Genres", keyColumn: "Name", keyValues: new object[]
            {
                "Programming", "Science Fiction", "Business", "Self Help",
                "History", "Fantasy", "Technology", "Biography"
            });
        }
    }
}

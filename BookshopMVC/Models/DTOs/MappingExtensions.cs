using BookshopMVC.Models;

namespace BookshopMVC.DTOs
{
    /// <summary>
    /// Extension methods for mapping between domain models and DTOs.
    /// These methods provide clean, reusable conversion logic to maintain separation of concerns
    /// between data models (for database) and DTOs (for API/UI).
    /// </summary>
    public static class MappingExtensions
    {
        #region Book Mappings

        /// <summary>
        /// Converts a Book entity to a complete BookDto with all properties.
        /// Used for detailed book views, edit forms, and full book information display.
        /// Includes category name for better UI experience without additional queries.
        /// </summary>
        /// <param name="book">The Book entity to convert</param>
        /// <returns>Complete BookDto with all book details and category name</returns>
        public static BookDto ToDto(this Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                ISBN = book.ISBN,
                Description = book.Description,
                Price = book.Price,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                CategoryId = book.CategoryId,
                CategoryName = book.Category?.Name
            };
        }

        /// <summary>
        /// Converts a Book entity to a lightweight BookSummaryDto.
        /// Used for book lists, search results, and card views where minimal data is needed.
        /// Improves performance by excluding heavy fields like description.
        /// Includes computed InStock property for UI display logic.
        /// </summary>
        /// <param name="book">The Book entity to convert</param>
        /// <returns>Lightweight BookSummaryDto for list displays</returns>
        public static BookSummaryDto ToSummaryDto(this Book book)
        {
            return new BookSummaryDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Price = book.Price,
                ImageUrl = book.ImageUrl,
                CategoryName = book.Category?.Name,
                InStock = book.Stock > 0
            };
        }

        /// <summary>
        /// Converts a CreateBookDto to a Book entity for database insertion.
        /// Used when creating new books from form submissions or API requests.
        /// Excludes Id (auto-generated) and navigation properties (set by EF Core).
        /// </summary>
        /// <param name="dto">The CreateBookDto from form/API</param>
        /// <returns>New Book entity ready for database insertion</returns>
        public static Book ToModel(this CreateBookDto dto)
        {
            return new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                ISBN = dto.ISBN,
                Description = dto.Description,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                CategoryId = dto.CategoryId
            };
        }

        /// <summary>
        /// Updates an existing Book entity with data from UpdateBookDto.
        /// Used for editing existing books while preserving Id and audit fields.
        /// Modifies the existing entity in-place for EF Core change tracking.
        /// </summary>
        /// <param name="dto">The UpdateBookDto with new values</param>
        /// <param name="book">The existing Book entity to update</param>
        public static void UpdateModel(this UpdateBookDto dto, Book book)
        {
            book.Title = dto.Title;
            book.Author = dto.Author;
            book.ISBN = dto.ISBN;
            book.Description = dto.Description;
            book.Price = dto.Price;
            book.Stock = dto.Stock;
            book.ImageUrl = dto.ImageUrl;
            book.CategoryId = dto.CategoryId;
        }

        #endregion

        #region Category Mappings

        /// <summary>
        /// Converts a Category entity to CategoryDto with book count.
        /// Used for category listings and dropdown selections.
        /// Includes computed BookCount for dashboard statistics without loading all books.
        /// </summary>
        /// <param name="category">The Category entity to convert</param>
        /// <returns>CategoryDto with basic info and book count</returns>
        public static CategoryDto ToDto(this Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                BookCount = category.Books?.Count ?? 0
            };
        }

        /// <summary>
        /// Converts a Category entity to CategoryWithBooksDto including all books.
        /// Used for category detail pages showing all books in the category.
        /// WARNING: Can be expensive for categories with many books - use with .Include() carefully.
        /// </summary>
        /// <param name="category">The Category entity with Books navigation property loaded</param>
        /// <returns>CategoryWithBooksDto containing all books in the category</returns>
        public static CategoryWithBooksDto ToWithBooksDto(this Category category)
        {
            return new CategoryWithBooksDto
            {
                Id = category.Id,
                Name = category.Name,
                Books = category.Books?.Select(b => b.ToSummaryDto()).ToList() ?? new List<BookSummaryDto>()
            };
        }

        /// <summary>
        /// Converts a CreateCategoryDto to a Category entity for database insertion.
        /// Used when creating new categories from admin forms or API requests.
        /// </summary>
        /// <param name="dto">The CreateCategoryDto from form/API</param>
        /// <returns>New Category entity ready for database insertion</returns>
        public static Category ToModel(this CreateCategoryDto dto)
        {
            return new Category
            {
                Name = dto.Name ?? throw new ArgumentNullException(nameof(dto.Name), "Category name is required")
            };
        }

        /// <summary>
        /// Updates an existing Category entity with data from UpdateCategoryDto.
        /// Used for editing existing categories while preserving Id and relationships.
        /// </summary>
        /// <param name="dto">The UpdateCategoryDto with new values</param>
        /// <param name="category">The existing Category entity to update</param>
        public static void UpdateModel(this UpdateCategoryDto dto, Category category)
        {
            category.Name = dto.Name ?? category.Name; // Keep existing if null
        }

        #endregion

        #region Customer Mappings

        /// <summary>
        /// Converts a Customer entity to CustomerDto excluding sensitive information.
        /// Used for customer profiles, admin customer lists, and order displays.
        /// Excludes PasswordHash for security and includes computed TotalOrders.
        /// </summary>
        /// <param name="customer">The Customer entity to convert</param>
        /// <returns>CustomerDto with safe customer information</returns>
        public static CustomerDto ToDto(this Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email,
                Address = customer.Address,
                TotalOrders = customer.Orders?.Count ?? 0
            };
        }

        /// <summary>
        /// Converts a Customer entity to minimal CustomerSummaryDto.
        /// Used for dropdowns, quick references, and lightweight customer displays.
        /// Contains only essential identification information.
        /// </summary>
        /// <param name="customer">The Customer entity to convert</param>
        /// <returns>Minimal CustomerSummaryDto with basic info</returns>
        public static CustomerSummaryDto ToSummaryDto(this Customer customer)
        {
            return new CustomerSummaryDto
            {
                Id = customer.Id,
                FullName = customer.FullName,
                Email = customer.Email
            };
        }

        /// <summary>
        /// Converts a CreateCustomerDto to a Customer entity for registration.
        /// Used during customer registration process with hashed password.
        /// Requires pre-hashed password for security - never store plain text passwords.
        /// </summary>
        /// <param name="dto">The CreateCustomerDto from registration form</param>
        /// <param name="passwordHash">Pre-hashed password from security service</param>
        /// <returns>New Customer entity ready for database insertion</returns>
        public static Customer ToModel(this CreateCustomerDto dto, string passwordHash)
        {
            return new Customer
            {
                FullName = dto.FullName ?? throw new ArgumentNullException(nameof(dto.FullName), "Full name is required"),
                Email = dto.Email ?? throw new ArgumentNullException(nameof(dto.Email), "Email is required"),
                PasswordHash = passwordHash,
                Address = dto.Address ?? string.Empty // Default to empty if null
            };
        }

        /// <summary>
        /// Updates an existing Customer entity with data from UpdateCustomerDto.
        /// Used for customer profile updates, excluding password changes.
        /// Password updates should use separate secure methods.
        /// </summary>
        /// <param name="dto">The UpdateCustomerDto with new values</param>
        /// <param name="customer">The existing Customer entity to update</param>
        public static void UpdateModel(this UpdateCustomerDto dto, Customer customer)
        {
            customer.FullName = dto.FullName ?? customer.FullName; // Keep existing if null
            customer.Email = dto.Email ?? customer.Email; // Keep existing if null
            customer.Address = dto.Address ?? customer.Address; // Keep existing if null
        }

        #endregion

        #region Order Mappings

        /// <summary>
        /// Converts an Order entity to complete OrderDto with customer and items.
        /// Used for order detail views, admin order management, and order confirmations.
        /// Includes customer information and all order items with computed totals.
        /// </summary>
        /// <param name="order">The Order entity with Customer and Items navigation properties loaded</param>
        /// <returns>Complete OrderDto with all order details</returns>
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.FullName ?? "Unknown Customer",
                CustomerEmail = order.Customer?.Email ?? "No Email",
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                TotalItems = order.Items?.Sum(i => i.Quantity) ?? 0,
                Items = order.Items?.Select(i => i.ToDto()).ToList() ?? new List<OrderItemDto>()
            };
        }

        /// <summary>
        /// Converts an Order entity to lightweight OrderSummaryDto.
        /// Used for order lists, customer order history, and admin dashboards.
        /// Excludes individual items for better performance in list views.
        /// </summary>
        /// <param name="order">The Order entity to convert</param>
        /// <returns>Lightweight OrderSummaryDto for list displays</returns>
        public static OrderSummaryDto ToSummaryDto(this Order order)
        {
            return new OrderSummaryDto
            {
                Id = order.Id,
                CustomerName = order.Customer?.FullName ?? "Unknown Customer",
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                TotalItems = order.Items?.Sum(i => i.Quantity) ?? 0
            };
        }

        /// <summary>
        /// Converts an Order entity to CustomerOrderHistoryDto for customer's order history.
        /// Used specifically for customer-facing order history pages.
        /// Includes item summaries but excludes sensitive customer data of others.
        /// </summary>
        /// <param name="order">The Order entity with Items navigation property loaded</param>
        /// <returns>CustomerOrderHistoryDto with order and item details</returns>
        public static CustomerOrderHistoryDto ToCustomerOrderHistoryDto(this Order order)
        {
            return new CustomerOrderHistoryDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                TotalItems = order.Items?.Sum(i => i.Quantity) ?? 0,
                Items = order.Items?.Select(i => i.ToSummaryDto()).ToList() ?? new List<OrderItemSummaryDto>()
            };
        }

        #endregion

        #region OrderItem Mappings

        /// <summary>
        /// Converts an OrderItem entity to complete OrderItemDto with book details.
        /// Used for order detail views and order management.
        /// Includes book information for display without additional queries.
        /// Computes TotalPrice (quantity × unit price) for UI convenience.
        /// </summary>
        /// <param name="orderItem">The OrderItem entity with Book navigation property loaded</param>
        /// <returns>Complete OrderItemDto with book details and computed total</returns>
        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                BookId = orderItem.BookId,
                BookTitle = orderItem.Book?.Title ?? "Unknown Book",
                BookAuthor = orderItem.Book?.Author ?? "Unknown Author",
                BookImageUrl = orderItem.Book?.ImageUrl ?? "",
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.Quantity * orderItem.UnitPrice
            };
        }

        /// <summary>
        /// Converts an OrderItem entity to OrderItemSummaryDto for concise displays.
        /// Used in order summaries, customer order history, and receipt views.
        /// Excludes OrderItem.Id and Order.Id for cleaner customer-facing displays.
        /// </summary>
        /// <param name="orderItem">The OrderItem entity with Book navigation property loaded</param>
        /// <returns>Concise OrderItemSummaryDto for customer displays</returns>
        public static OrderItemSummaryDto ToSummaryDto(this OrderItem orderItem)
        {
            return new OrderItemSummaryDto
            {
                BookId = orderItem.BookId,
                BookTitle = orderItem.Book?.Title ?? "Unknown Book",
                BookAuthor = orderItem.Book?.Author ?? "Unknown Author",
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.Quantity * orderItem.UnitPrice
            };
        }

        /// <summary>
        /// Converts a Book entity to CartItemDto for shopping cart display.
        /// Used for shopping cart functionality before order creation.
        /// Includes current stock information for inventory validation.
        /// Computes total price for cart totals and displays current availability.
        /// </summary>
        /// <param name="book">The Book entity to add to cart</param>
        /// <param name="quantity">The desired quantity for the cart</param>
        /// <returns>CartItemDto with book details, quantity, and stock info</returns>
        public static CartItemDto ToCartItemDto(this Book book, int quantity)
        {
            return new CartItemDto
            {
                BookId = book.Id,
                BookTitle = book.Title,
                BookAuthor = book.Author,
                BookImageUrl = book.ImageUrl ?? "",
                BookPrice = book.Price,
                Quantity = quantity,
                TotalPrice = book.Price * quantity,
                AvailableStock = book.Stock
            };
        }

        #endregion
    }
}

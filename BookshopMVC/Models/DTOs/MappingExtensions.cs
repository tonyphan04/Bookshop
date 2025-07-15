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
        #region User Mappings

        /// <summary>
        /// Converts a User entity to UserDto with order statistics.
        /// Used for user profiles and admin user management.
        /// </summary>
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                Role = user.Role,
                IsActive = user.IsActive,
                RegistrationDate = user.RegistrationDate,
                IsCustomer = user.IsCustomer,
                IsAdmin = user.IsAdmin,
                OrderCount = user.Orders?.Count ?? 0,
                TotalSpent = user.Orders?.Sum(o => o.TotalPrice) ?? 0
            };
        }

        /// <summary>
        /// Converts a User entity to UserSummaryDto for lists.
        /// </summary>
        public static UserSummaryDto ToSummaryDto(this User user)
        {
            return new UserSummaryDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                RegistrationDate = user.RegistrationDate
            };
        }

        /// <summary>
        /// Converts a User entity to UserAuthDto for authentication.
        /// </summary>
        public static UserAuthDto ToAuthDto(this User user)
        {
            return new UserAuthDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        /// <summary>
        /// Converts CreateUserDto to User entity.
        /// </summary>
        public static User ToEntity(this CreateUserDto dto)
        {
            return new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
                Address = dto.Address,
                Role = dto.Role,
                IsActive = true,
                RegistrationDate = DateTime.UtcNow
                // PasswordHash should be set separately with proper hashing
            };
        }

        /// <summary>
        /// Updates an existing User entity with values from UpdateUserDto.
        /// </summary>
        public static void UpdateFromDto(this User user, UpdateUserDto dto)
        {
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.Address = dto.Address;
            
            // Only update role and active status if provided (admin only)
            if (dto.Role.HasValue)
                user.Role = dto.Role.Value;
            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;
        }

        #endregion

        #region Author Mappings

        /// <summary>
        /// Converts an Author entity to AuthorDto with book statistics.
        /// </summary>
        public static AuthorDto ToDto(this Author author)
        {
            return new AuthorDto
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                FullName = author.FullName,
                Biography = author.Biography,
                CreatedDate = author.CreatedDate,
                BookCount = author.AuthorBooks?.Count ?? 0,
                Books = author.AuthorBooks?.Select(ab => ab.Book.ToSummaryDto()).ToList() ?? new List<BookSummaryDto>()
            };
        }

        /// <summary>
        /// Converts an Author entity to AuthorSummaryDto for lists.
        /// </summary>
        public static AuthorSummaryDto ToSummaryDto(this Author author)
        {
            return new AuthorSummaryDto
            {
                Id = author.Id,
                FullName = author.FullName,
                BookCount = author.AuthorBooks?.Count ?? 0
            };
        }

        /// <summary>
        /// Converts CreateAuthorDto to Author entity.
        /// </summary>
        public static Author ToEntity(this CreateAuthorDto dto)
        {
            return new Author
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Biography = dto.Biography,
                CreatedDate = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Updates an existing Author entity with values from UpdateAuthorDto.
        /// </summary>
        public static void UpdateFromDto(this Author author, UpdateAuthorDto dto)
        {
            author.FirstName = dto.FirstName;
            author.LastName = dto.LastName;
            author.Biography = dto.Biography;
        }

        #endregion

        #region Genre Mappings

        /// <summary>
        /// Converts a Genre entity to GenreDto with book statistics.
        /// </summary>
        public static GenreDto ToDto(this Genre genre)
        {
            return new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name,
                Description = genre.Description,
                IsActive = genre.IsActive,
                DisplayOrder = genre.DisplayOrder,
                BookCount = genre.Books?.Count ?? 0,
                Books = genre.Books?.Where(b => b.IsActive).Select(b => b.ToSummaryDto()).ToList() ?? new List<BookSummaryDto>()
            };
        }

        /// <summary>
        /// Converts a Genre entity to GenreSummaryDto for lists.
        /// </summary>
        public static GenreSummaryDto ToSummaryDto(this Genre genre)
        {
            return new GenreSummaryDto
            {
                Id = genre.Id,
                Name = genre.Name,
                IsActive = genre.IsActive,
                DisplayOrder = genre.DisplayOrder,
                BookCount = genre.Books?.Where(b => b.IsActive).Count() ?? 0
            };
        }

        /// <summary>
        /// Converts CreateGenreDto to Genre entity.
        /// </summary>
        public static Genre ToEntity(this CreateGenreDto dto)
        {
            return new Genre
            {
                Name = dto.Name,
                Description = dto.Description,
                IsActive = dto.IsActive,
                DisplayOrder = dto.DisplayOrder
            };
        }

        /// <summary>
        /// Updates an existing Genre entity with values from UpdateGenreDto.
        /// </summary>
        public static void UpdateFromDto(this Genre genre, UpdateGenreDto dto)
        {
            genre.Name = dto.Name;
            genre.Description = dto.Description;
            genre.IsActive = dto.IsActive;
            genre.DisplayOrder = dto.DisplayOrder;
        }

        #endregion

        #region Book Mappings

        /// <summary>
        /// Converts a Book entity to a complete BookDto with all properties.
        /// Used for detailed book views, edit forms, and full book information display.
        /// Includes genre name and authors to avoid additional database queries.
        /// </summary>
        public static BookDto ToDto(this Book book)
        {
            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN13 = book.ISBN13,
                Price = book.Price,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                GenreId = book.GenreId,
                GenreName = book.Genre?.Name,
                Publisher = book.Publisher,
                Language = book.Language,
                IsActive = book.IsActive,
                CreatedAt = book.CreatedDate,
                // Authors will be populated separately if needed
                Authors = book.AuthorBooks?.OrderBy(ab => ab.AuthorOrder).Select(ab => ab.Author.FullName).ToList() ?? new List<string>()
            };
        }

        /// <summary>
        /// Converts a Book entity to a lightweight BookSummaryDto.
        /// Used for book lists, search results, and card views where minimal data is needed.
        /// Improves performance by excluding heavy fields.
        /// Includes computed InStock property for UI display logic.
        /// </summary>
        public static BookSummaryDto ToSummaryDto(this Book book)
        {
            return new BookSummaryDto
            {
                Id = book.Id,
                Title = book.Title,
                Price = book.Price,
                ImageUrl = book.ImageUrl,
                GenreName = book.Genre?.Name,
                InStock = book.Stock > 0,
                Authors = book.AuthorBooks?.OrderBy(ab => ab.AuthorOrder).Select(ab => ab.Author.FullName).ToList() ?? new List<string>()
            };
        }

        /// <summary>
        /// Converts a BookDto back to a Book entity for create/update operations.
        /// Maps all DTO properties to corresponding entity properties.
        /// Note: Navigation properties (Genre, AuthorBooks) are not set and should be handled separately.
        /// </summary>
        public static Book ToEntity(this BookDto dto)
        {
            return new Book
            {
                Id = dto.Id,
                Title = dto.Title,
                ISBN13 = dto.ISBN13,
                Price = dto.Price,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                GenreId = dto.GenreId,
                Publisher = dto.Publisher,
                Language = dto.Language,
                IsActive = dto.IsActive,
                CreatedDate = dto.CreatedAt == default ? DateTime.UtcNow : dto.CreatedAt
            };
        }

        /// <summary>
        /// Updates an existing Book entity with values from a BookDto.
        /// Used for update operations to preserve entity tracking and navigation properties.
        /// </summary>
        public static void UpdateFromDto(this Book book, BookDto dto)
        {
            book.Title = dto.Title;
            book.ISBN13 = dto.ISBN13;
            book.Price = dto.Price;
            book.Stock = dto.Stock;
            book.ImageUrl = dto.ImageUrl;
            book.GenreId = dto.GenreId;
            book.Publisher = dto.Publisher;
            book.Language = dto.Language;
            book.IsActive = dto.IsActive;
        }

        #endregion

        #region Order Mappings

        /// <summary>
        /// Converts an Order entity to OrderDto with basic order information.
        /// Used for order listings and summary displays.
        /// Includes user name and item count for better UX.
        /// </summary>
        public static OrderDto ToDto(this Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.FullName,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedDate,
                ItemCount = order.Items?.Count ?? 0
            };
        }

        /// <summary>
        /// Converts an Order entity to detailed OrderWithItemsDto including all order items.
        /// Used for order detail pages showing complete order information.
        /// WARNING: Can be expensive for orders with many items - use with .Include() carefully.
        /// </summary>
        public static OrderWithItemsDto ToWithItemsDto(this Order order)
        {
            return new OrderWithItemsDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.FullName,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedDate,
                OrderItems = order.Items?.Select(oi => oi.ToDto()).ToList() ?? new List<OrderItemDto>()
            };
        }

        /// <summary>
        /// Converts an OrderDto back to an Order entity for create/update operations.
        /// Maps DTO properties to entity properties.
        /// Note: Navigation properties are not set and should be handled separately.
        /// </summary>
        public static Order ToEntity(this OrderDto dto)
        {
            return new Order
            {
                Id = dto.Id,
                UserId = dto.UserId,
                OrderDate = dto.OrderDate,
                TotalPrice = dto.TotalPrice,
                Status = Enum.Parse<OrderStatus>(dto.Status),
                CreatedDate = DateTime.UtcNow
            };
        }

        #endregion

        #region OrderItem Mappings

        /// <summary>
        /// Converts an OrderItem entity to OrderItemDto.
        /// Used in order details and cart displays.
        /// Includes book title and calculated total for better UX.
        /// </summary>
        public static OrderItemDto ToDto(this OrderItem orderItem)
        {
            return new OrderItemDto
            {
                Id = orderItem.Id,
                OrderId = orderItem.OrderId,
                BookId = orderItem.BookId,
                BookTitle = orderItem.Book?.Title,
                Quantity = orderItem.Quantity,
                UnitPrice = orderItem.UnitPrice,
                TotalPrice = orderItem.TotalPrice
            };
        }

        /// <summary>
        /// Converts an OrderItemDto back to an OrderItem entity for create operations.
        /// Maps DTO properties to entity properties.
        /// Note: Navigation properties are not set and should be handled separately.
        /// TotalPrice is computed automatically from Quantity * UnitPrice.
        /// </summary>
        public static OrderItem ToEntity(this OrderItemDto dto)
        {
            return new OrderItem
            {
                Id = dto.Id,
                OrderId = dto.OrderId,
                BookId = dto.BookId,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice
                // TotalPrice is computed automatically
            };
        }

        #endregion

        #region CartItem Mappings

        /// <summary>
        /// Converts a CartItem entity to CartItemDto.
        /// Used for cart displays and management.
        /// Includes book details and calculated total for better UX.
        /// </summary>
        public static CartItemDto ToDto(this CartItem cartItem)
        {
            return new CartItemDto
            {
                Id = cartItem.Id,
                UserId = cartItem.UserId,
                BookId = cartItem.BookId,
                BookTitle = cartItem.Book?.Title ?? string.Empty,
                BookImageUrl = cartItem.Book?.ImageUrl,
                BookPrice = cartItem.Book?.Price ?? 0,
                Quantity = cartItem.Quantity,
                TotalPrice = cartItem.TotalPrice,
                AddedDate = cartItem.AddedDate,
                AvailableStock = cartItem.Book?.Stock ?? 0
            };
        }

        /// <summary>
        /// Converts AddToCartDto to CartItem entity.
        /// </summary>
        public static CartItem ToEntity(this AddToCartDto dto, int userId)
        {
            return new CartItem
            {
                UserId = userId,
                BookId = dto.BookId,
                Quantity = dto.Quantity,
                AddedDate = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Updates an existing CartItem entity with new quantity.
        /// </summary>
        public static void UpdateFromDto(this CartItem cartItem, UpdateCartItemDto dto)
        {
            cartItem.Quantity = dto.Quantity;
        }

        /// <summary>
        /// Converts a CartItem to an OrderItem during checkout process.
        /// This is the core business logic for cart-to-order conversion.
        /// </summary>
        public static OrderItem ToOrderItem(this CartItem cartItem, int orderId)
        {
            return new OrderItem
            {
                OrderId = orderId,
                BookId = cartItem.BookId,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Book?.Price ?? 0, // Capture current price at time of order
                Book = cartItem.Book // Include navigation property if loaded
                // TotalPrice is computed automatically (Quantity * UnitPrice)
            };
        }

        /// <summary>
        /// Converts multiple CartItems to OrderItems during checkout process.
        /// </summary>
        public static List<OrderItem> ToOrderItems(this IEnumerable<CartItem> cartItems, int orderId)
        {
            return cartItems.Select(ci => ci.ToOrderItem(orderId)).ToList();
        }

        #endregion
    }
}
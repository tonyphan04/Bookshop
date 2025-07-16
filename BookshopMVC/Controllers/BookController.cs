using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;

namespace BookshopMVC.Controllers
{
    /// <summary>
    /// Controller for managing book CRUD operations.
    /// Handles both MVC views and API endpoints for book management.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region READ Operations

        /// <summary>
        /// GET: api/Book
        /// Retrieves all books as summary DTOs for list displays
        /// </summary>
        /// <returns>List of BookSummaryDto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookSummaryDto>>> GetBooks()
        {
            try
            {
                var books = await _context.Books
                    .Include(b => b.Genre)
                    .Include(b => b.AuthorBooks)
                        .ThenInclude(ab => ab.Author)
                    .Where(b => b.IsActive)
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                var bookSummaries = books.Select(book => new BookSummaryDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    ImageUrl = book.ImageUrl,
                    GenreName = book.Genre.Name,
                    InStock = book.Stock > 0,
                    Authors = book.AuthorBooks
                        .OrderBy(ab => ab.AuthorOrder)
                        .Select(ab => $"{ab.Author.FirstName} {ab.Author.LastName}")
                        .ToList()
                }).ToList();

                return Ok(bookSummaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving books: {ex.Message}");
            }
        }

        /// <summary>
        /// GET: api/Book/5
        /// Retrieves a specific book by ID with full details
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>BookDto with complete book information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetBook(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Genre)
                    .Include(b => b.AuthorBooks)
                        .ThenInclude(ab => ab.Author)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                var bookDto = new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    ISBN13 = book.ISBN13,
                    Price = book.Price,
                    Stock = book.Stock,
                    ImageUrl = book.ImageUrl,
                    GenreId = book.GenreId,
                    Publisher = book.Publisher ?? string.Empty,
                    Language = book.Language,
                    IsActive = book.IsActive,
                    CreatedAt = book.CreatedDate,
                    GenreName = book.Genre.Name,
                    Authors = book.AuthorBooks
                        .OrderBy(ab => ab.AuthorOrder)
                        .Select(ab => $"{ab.Author.FirstName} {ab.Author.LastName}")
                        .ToList()
                };

                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving book: {ex.Message}");
            }
        }

        /// <summary>
        /// GET: api/Book/genre/5
        /// Retrieves books by genre ID
        /// </summary>
        /// <param name="genreId">Genre ID</param>
        /// <returns>List of BookSummaryDto for the specified genre</returns>
        [HttpGet("genre/{genreId}")]
        public async Task<ActionResult<IEnumerable<BookSummaryDto>>> GetBooksByGenre(int genreId)
        {
            try
            {
                // Validate genre exists
                var genreExists = await _context.Genres.AnyAsync(g => g.Id == genreId);
                if (!genreExists)
                {
                    return NotFound($"Genre with ID {genreId} not found");
                }

                var books = await _context.Books
                    .Include(b => b.Genre)
                    .Include(b => b.AuthorBooks)
                        .ThenInclude(ab => ab.Author)
                    .Where(b => b.GenreId == genreId && b.IsActive)
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                var bookSummaries = books.Select(book => new BookSummaryDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    ImageUrl = book.ImageUrl,
                    GenreName = book.Genre.Name,
                    InStock = book.Stock > 0,
                    Authors = book.AuthorBooks
                        .OrderBy(ab => ab.AuthorOrder)
                        .Select(ab => $"{ab.Author.FirstName} {ab.Author.LastName}")
                        .ToList()
                }).ToList();

                return Ok(bookSummaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving books by genre: {ex.Message}");
            }
        }

        /// <summary>
        /// GET: api/Book/search?query=programming
        /// Search books by title, author, or description
        /// </summary>
        /// <param name="query">Search query string</param>
        /// <returns>List of BookSummaryDto matching the search</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<BookSummaryDto>>> SearchBooks([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Search query cannot be empty");
                }

                var searchTerm = query.ToLower().Trim();

                var books = await _context.Books
                    .Include(b => b.Genre)
                    .Include(b => b.AuthorBooks)
                        .ThenInclude(ab => ab.Author)
                    .Where(b => b.IsActive && (
                        b.Title.ToLower().Contains(searchTerm) ||
                        (b.Description != null && b.Description.ToLower().Contains(searchTerm)) ||
                        b.AuthorBooks.Any(ab =>
                            ab.Author.FirstName.ToLower().Contains(searchTerm) ||
                            ab.Author.LastName.ToLower().Contains(searchTerm))
                    ))
                    .OrderBy(b => b.Title)
                    .ToListAsync();

                var bookSummaries = books.Select(book => new BookSummaryDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Price = book.Price,
                    ImageUrl = book.ImageUrl,
                    GenreName = book.Genre.Name,
                    InStock = book.Stock > 0,
                    Authors = book.AuthorBooks
                        .OrderBy(ab => ab.AuthorOrder)
                        .Select(ab => $"{ab.Author.FirstName} {ab.Author.LastName}")
                        .ToList()
                }).ToList();

                return Ok(bookSummaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error searching books: {ex.Message}");
            }
        }

        #endregion

        #region CREATE Operations

        /// <summary>
        /// POST: api/Book
        /// Creates a new book
        /// </summary>
        /// <param name="createBookDto">Book creation data</param>
        /// <returns>Created BookDto</returns>
        [HttpPost]
        public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
        {
            try
            {
                // Validate input
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check ISBN13 uniqueness
                if (!await IsIsbn13Unique(createBookDto.ISBN13))
                {
                    return BadRequest($"ISBN13 {createBookDto.ISBN13} already exists");
                }

                // Validate genre exists
                if (!await ValidateGenreId(createBookDto.GenreId))
                {
                    return BadRequest($"Genre with ID {createBookDto.GenreId} not found");
                }

                // Validate authors exist
                if (createBookDto.AuthorIds.Any() && !await ValidateAuthorIds(createBookDto.AuthorIds))
                {
                    return BadRequest("One or more author IDs are invalid");
                }

                // Create book entity
                var book = new Book
                {
                    Title = createBookDto.Title,
                    ISBN13 = createBookDto.ISBN13,
                    Price = createBookDto.Price,
                    Stock = createBookDto.Stock,
                    ImageUrl = createBookDto.ImageUrl,
                    GenreId = createBookDto.GenreId,
                    Publisher = createBookDto.Publisher,
                    Language = createBookDto.Language,
                    IsActive = createBookDto.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                // Create author-book relationships
                if (createBookDto.AuthorIds.Any())
                {
                    var authorBooks = createBookDto.AuthorIds.Select((authorId, index) => new AuthorBook
                    {
                        AuthorId = authorId,
                        BookId = book.Id,
                        AuthorOrder = index + 1
                    }).ToList();

                    _context.AuthorBooks.AddRange(authorBooks);
                    await _context.SaveChangesAsync();
                }

                // Return created book
                var createdBook = await GetBookByIdInternal(book.Id);
                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, createdBook);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating book: {ex.Message}");
            }
        }

        #endregion

        #region UPDATE Operations

        /// <summary>
        /// PUT: api/Book/5
        /// Updates an existing book
        /// </summary>
        /// <param name="id">Book ID to update</param>
        /// <param name="updateBookDto">Updated book data</param>
        /// <returns>NoContent if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var book = await _context.Books
                    .Include(b => b.AuthorBooks)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                // Check ISBN13 uniqueness (excluding current book)
                if (!await IsIsbn13Unique(updateBookDto.ISBN13, id))
                {
                    return BadRequest($"ISBN13 {updateBookDto.ISBN13} already exists");
                }

                // Validate genre exists
                if (!await ValidateGenreId(updateBookDto.GenreId))
                {
                    return BadRequest($"Genre with ID {updateBookDto.GenreId} not found");
                }

                // Validate authors exist
                if (updateBookDto.AuthorIds.Any() && !await ValidateAuthorIds(updateBookDto.AuthorIds))
                {
                    return BadRequest("One or more author IDs are invalid");
                }

                // Update book properties
                book.Title = updateBookDto.Title;
                book.ISBN13 = updateBookDto.ISBN13;
                book.Price = updateBookDto.Price;
                book.Stock = updateBookDto.Stock;
                book.ImageUrl = updateBookDto.ImageUrl;
                book.GenreId = updateBookDto.GenreId;
                book.Publisher = updateBookDto.Publisher;
                book.Language = updateBookDto.Language;
                book.IsActive = updateBookDto.IsActive;

                // Update author relationships
                _context.AuthorBooks.RemoveRange(book.AuthorBooks);

                if (updateBookDto.AuthorIds.Any())
                {
                    var newAuthorBooks = updateBookDto.AuthorIds.Select((authorId, index) => new AuthorBook
                    {
                        AuthorId = authorId,
                        BookId = book.Id,
                        AuthorOrder = index + 1
                    }).ToList();

                    _context.AuthorBooks.AddRange(newAuthorBooks);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating book: {ex.Message}");
            }
        }

        /// <summary>
        /// PATCH: api/Book/5/stock
        /// Updates only the stock quantity of a book
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <param name="newStock">New stock quantity</param>
        /// <returns>NoContent if successful</returns>
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateBookStock(int id, [FromBody] int newStock)
        {
            try
            {
                if (newStock < 0)
                {
                    return BadRequest("Stock quantity cannot be negative");
                }

                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                book.Stock = newStock;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating book stock: {ex.Message}");
            }
        }

        /// <summary>
        /// PATCH: api/Book/5/status
        /// Toggles the active status of a book (soft delete)
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>NoContent if successful</returns>
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> ToggleBookStatus(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                book.IsActive = !book.IsActive;
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error toggling book status: {ex.Message}");
            }
        }

        #endregion

        #region DELETE Operations

        /// <summary>
        /// DELETE: api/Book/5
        /// Soft deletes a book (sets IsActive = false)
        /// </summary>
        /// <param name="id">Book ID to delete</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                // Check if book has existing orders (business rule)
                var hasOrders = await _context.OrderItems.AnyAsync(oi => oi.BookId == id);
                if (hasOrders)
                {
                    // If book has orders, only soft delete
                    book.IsActive = false;
                }
                else
                {
                    // If no orders, can safely soft delete
                    book.IsActive = false;
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting book: {ex.Message}");
            }
        }

        /// <summary>
        /// DELETE: api/Book/5/permanent
        /// Permanently deletes a book and all related data
        /// WARNING: This is destructive and should be admin-only
        /// </summary>
        /// <param name="id">Book ID to permanently delete</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> PermanentDeleteBook(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.AuthorBooks)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound($"Book with ID {id} not found");
                }

                // Check for existing OrderItems (prevent deletion if has order history)
                var hasOrderItems = await _context.OrderItems.AnyAsync(oi => oi.BookId == id);
                if (hasOrderItems)
                {
                    return BadRequest("Cannot permanently delete book with existing order history. Use soft delete instead.");
                }

                // Check for existing CartItems
                var hasCartItems = await _context.CartItems.AnyAsync(ci => ci.BookId == id);
                if (hasCartItems)
                {
                    // Remove from all carts first
                    var cartItems = await _context.CartItems.Where(ci => ci.BookId == id).ToListAsync();
                    _context.CartItems.RemoveRange(cartItems);
                }

                // Remove author relationships
                _context.AuthorBooks.RemoveRange(book.AuthorBooks);

                // Remove the book
                _context.Books.Remove(book);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error permanently deleting book: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Internal method to get book by ID with all related data
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>BookDto or null if not found</returns>
        private async Task<BookDto?> GetBookByIdInternal(int id)
        {
            var book = await _context.Books
                .Include(b => b.Genre)
                .Include(b => b.AuthorBooks)
                    .ThenInclude(ab => ab.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return null;

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                ISBN13 = book.ISBN13,
                Price = book.Price,
                Stock = book.Stock,
                ImageUrl = book.ImageUrl,
                GenreId = book.GenreId,
                Publisher = book.Publisher ?? string.Empty,
                Language = book.Language,
                IsActive = book.IsActive,
                CreatedAt = book.CreatedDate,
                GenreName = book.Genre.Name,
                Authors = book.AuthorBooks
                    .OrderBy(ab => ab.AuthorOrder)
                    .Select(ab => $"{ab.Author.FirstName} {ab.Author.LastName}")
                    .ToList()
            };
        }

        /// <summary>
        /// Checks if a book exists in the database
        /// </summary>
        /// <param name="id">Book ID</param>
        /// <returns>True if book exists</returns>
        private async Task<bool> BookExists(int id)
        {
            return await _context.Books.AnyAsync(b => b.Id == id);
        }

        /// <summary>
        /// Validates that all author IDs exist in the database
        /// </summary>
        /// <param name="authorIds">List of author IDs to validate</param>
        /// <returns>True if all authors exist</returns>
        private async Task<bool> ValidateAuthorIds(List<int> authorIds)
        {
            if (!authorIds.Any()) return true;

            var existingAuthorCount = await _context.Authors
                .Where(a => authorIds.Contains(a.Id))
                .CountAsync();

            return existingAuthorCount == authorIds.Count;
        }

        /// <summary>
        /// Validates that genre ID exists in the database
        /// </summary>
        /// <param name="genreId">Genre ID to validate</param>
        /// <returns>True if genre exists</returns>
        private async Task<bool> ValidateGenreId(int genreId)
        {
            return await _context.Genres.AnyAsync(g => g.Id == genreId);
        }

        /// <summary>
        /// Checks if ISBN13 is already in use by another book
        /// </summary>
        /// <param name="isbn13">ISBN13 to check</param>
        /// <param name="excludeBookId">Book ID to exclude from check (for updates)</param>
        /// <returns>True if ISBN13 is unique</returns>
        private async Task<bool> IsIsbn13Unique(string isbn13, int? excludeBookId = null)
        {
            var query = _context.Books.Where(b => b.ISBN13 == isbn13);

            if (excludeBookId.HasValue)
            {
                query = query.Where(b => b.Id != excludeBookId.Value);
            }

            return !await query.AnyAsync();
        }

        #endregion
    }
}

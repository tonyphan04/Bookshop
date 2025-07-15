using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.Data;
using BookshopMVC.Models;
using BookshopMVC.DTOs;

namespace BookshopMVC.Controllers
{
    /// <summary>
    /// API Controller for managing books in the bookshop.
    /// Uses DTOs for clean separation between data models and API responses.
    /// Provides CRUD operations with proper validation and error handling.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all books with pagination and optional search functionality.
        /// Returns lightweight BookSummaryDto for better performance in list views.
        /// </summary>
        /// <param name="request">Search and pagination parameters</param>
        /// <returns>Paginated list of book summaries</returns>
        [HttpGet]
        public async Task<ActionResult<PaginatedResponse<BookSummaryDto>>> GetBooks([FromQuery] BookSearchRequest request)
        {
            try
            {
                var query = _context.Books.Include(b => b.Category).AsQueryable();

                // Apply search filters
                if (!string.IsNullOrEmpty(request.Query))
                {
                    query = query.Where(b => b.Title.Contains(request.Query) ||
                                           b.Author.Contains(request.Query) ||
                                           b.ISBN.Contains(request.Query));
                }

                if (request.CategoryId.HasValue)
                {
                    query = query.Where(b => b.CategoryId == request.CategoryId.Value);
                }

                if (request.MinPrice.HasValue)
                {
                    query = query.Where(b => b.Price >= request.MinPrice.Value);
                }

                if (request.MaxPrice.HasValue)
                {
                    query = query.Where(b => b.Price <= request.MaxPrice.Value);
                }

                if (request.InStockOnly.HasValue && request.InStockOnly.Value)
                {
                    query = query.Where(b => b.Stock > 0);
                }

                // Apply sorting
                query = request.SortBy?.ToLower() switch
                {
                    "title" => request.SortDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
                    "author" => request.SortDescending ? query.OrderByDescending(b => b.Author) : query.OrderBy(b => b.Author),
                    "price" => request.SortDescending ? query.OrderByDescending(b => b.Price) : query.OrderBy(b => b.Price),
                    _ => request.SortDescending ? query.OrderByDescending(b => b.Id) : query.OrderBy(b => b.Id)
                };

                // Get total count for pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var books = await query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(b => b.ToSummaryDto())
                    .ToListAsync();

                var response = new PaginatedResponse<BookSummaryDto>(books, request.Page, request.PageSize, totalCount);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PaginatedResponse<BookSummaryDto>>.ErrorResponse(
                    "An error occurred while retrieving books", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Gets a specific book by ID with complete details.
        /// Returns full BookDto with all book information and category details.
        /// </summary>
        /// <param name="id">The book ID</param>
        /// <returns>Complete book details or NotFound if book doesn't exist</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BookDto>>> GetBook(int id)
        {
            try
            {
                var book = await _context.Books
                    .Include(b => b.Category)
                    .FirstOrDefaultAsync(b => b.Id == id);

                if (book == null)
                {
                    return NotFound(ApiResponse<BookDto>.ErrorResponse("Book not found"));
                }

                var bookDto = book.ToDto();
                return Ok(ApiResponse<BookDto>.SuccessResponse(bookDto, "Book retrieved successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<BookDto>.ErrorResponse(
                    "An error occurred while retrieving the book", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Creates a new book from the provided CreateBookDto.
        /// Validates the input and returns the created book with generated ID.
        /// </summary>
        /// <param name="createBookDto">Book creation data with validation</param>
        /// <returns>Created book details or validation errors</returns>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<BookDto>>> CreateBook(CreateBookDto createBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<BookDto>.ErrorResponse("Validation failed", errors));
                }

                // Check if category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == createBookDto.CategoryId);
                if (!categoryExists)
                {
                    return BadRequest(ApiResponse<BookDto>.ErrorResponse("Invalid category ID"));
                }

                // Check if ISBN already exists
                var isbnExists = await _context.Books.AnyAsync(b => b.ISBN == createBookDto.ISBN);
                if (isbnExists)
                {
                    return BadRequest(ApiResponse<BookDto>.ErrorResponse("A book with this ISBN already exists"));
                }

                var book = createBookDto.ToModel();
                _context.Books.Add(book);
                await _context.SaveChangesAsync();

                // Reload book with category for response
                var createdBook = await _context.Books
                    .Include(b => b.Category)
                    .FirstAsync(b => b.Id == book.Id);

                var bookDto = createdBook.ToDto();
                return CreatedAtAction(nameof(GetBook), new { id = book.Id },
                    ApiResponse<BookDto>.SuccessResponse(bookDto, "Book created successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<BookDto>.ErrorResponse(
                    "An error occurred while creating the book", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Updates an existing book with the provided UpdateBookDto.
        /// Validates the input and updates only the provided fields.
        /// </summary>
        /// <param name="id">The book ID to update</param>
        /// <param name="updateBookDto">Updated book data with validation</param>
        /// <returns>Success message or validation/not found errors</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<BookDto>>> UpdateBook(int id, UpdateBookDto updateBookDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(ApiResponse<BookDto>.ErrorResponse("Validation failed", errors));
                }

                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound(ApiResponse<BookDto>.ErrorResponse("Book not found"));
                }

                // Check if category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == updateBookDto.CategoryId);
                if (!categoryExists)
                {
                    return BadRequest(ApiResponse<BookDto>.ErrorResponse("Invalid category ID"));
                }

                // Check if ISBN already exists for another book
                var isbnExists = await _context.Books.AnyAsync(b => b.ISBN == updateBookDto.ISBN && b.Id != id);
                if (isbnExists)
                {
                    return BadRequest(ApiResponse<BookDto>.ErrorResponse("A book with this ISBN already exists"));
                }

                // Update the book using the mapping extension
                updateBookDto.UpdateModel(book);
                await _context.SaveChangesAsync();

                // Reload book with category for response
                var updatedBook = await _context.Books
                    .Include(b => b.Category)
                    .FirstAsync(b => b.Id == id);

                var bookDto = updatedBook.ToDto();
                return Ok(ApiResponse<BookDto>.SuccessResponse(bookDto, "Book updated successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<BookDto>.ErrorResponse(
                    "An error occurred while updating the book", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Deletes a book by ID.
        /// Checks for dependencies and prevents deletion if book is referenced in orders.
        /// </summary>
        /// <param name="id">The book ID to delete</param>
        /// <returns>Success message or not found/conflict errors</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    return NotFound(ApiResponse<string>.ErrorResponse("Book not found"));
                }

                // Check if book is referenced in any orders
                var hasOrders = await _context.OrderItems.AnyAsync(oi => oi.BookId == id);
                if (hasOrders)
                {
                    return Conflict(ApiResponse<string>.ErrorResponse(
                        "Cannot delete book because it is referenced in one or more orders"));
                }

                _context.Books.Remove(book);
                await _context.SaveChangesAsync();

                return Ok(ApiResponse<string>.SuccessResponse("Book deleted successfully", "Book deleted successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<string>.ErrorResponse(
                    "An error occurred while deleting the book", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Gets books by category with pagination.
        /// Returns lightweight BookSummaryDto for category browsing.
        /// </summary>
        /// <param name="categoryId">The category ID</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated list of books in the specified category</returns>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<PaginatedResponse<BookSummaryDto>>> GetBooksByCategory(
            int categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                // Check if category exists
                var categoryExists = await _context.Categories.AnyAsync(c => c.Id == categoryId);
                if (!categoryExists)
                {
                    return NotFound(ApiResponse<PaginatedResponse<BookSummaryDto>>.ErrorResponse("Category not found"));
                }

                var query = _context.Books
                    .Include(b => b.Category)
                    .Where(b => b.CategoryId == categoryId)
                    .OrderBy(b => b.Title);

                var totalCount = await query.CountAsync();

                var books = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => b.ToSummaryDto())
                    .ToListAsync();

                var response = new PaginatedResponse<BookSummaryDto>(books, page, pageSize, totalCount);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PaginatedResponse<BookSummaryDto>>.ErrorResponse(
                    "An error occurred while retrieving books by category", new List<string> { ex.Message }));
            }
        }

        /// <summary>
        /// Searches books by title, author, or ISBN with pagination.
        /// Returns lightweight BookSummaryDto optimized for search results.
        /// </summary>
        /// <param name="searchTerm">Search term to match against title, author, or ISBN</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Items per page (default: 10)</param>
        /// <returns>Paginated search results</returns>
        [HttpGet("search")]
        public async Task<ActionResult<PaginatedResponse<BookSummaryDto>>> SearchBooks(
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return BadRequest(ApiResponse<PaginatedResponse<BookSummaryDto>>.ErrorResponse("Search term is required"));
                }

                var query = _context.Books
                    .Include(b => b.Category)
                    .Where(b => b.Title.Contains(searchTerm) ||
                               b.Author.Contains(searchTerm) ||
                               b.ISBN.Contains(searchTerm))
                    .OrderBy(b => b.Title);

                var totalCount = await query.CountAsync();

                var books = await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(b => b.ToSummaryDto())
                    .ToListAsync();

                var response = new PaginatedResponse<BookSummaryDto>(books, page, pageSize, totalCount);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<PaginatedResponse<BookSummaryDto>>.ErrorResponse(
                    "An error occurred while searching books", new List<string> { ex.Message }));
            }
        }
    }
}

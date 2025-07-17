using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace BookshopMVC.Controllers
{
    // Controller for managing author CRUD operations and book relationships
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AuthorController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        #region READ Operations

        // GET: api/Author - Retrieves all authors ordered by name
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthors()
        {
            try
            {
                // Use AutoMapper with ProjectTo for efficient database projection
                var authors = await _context.Authors
                    .OrderBy(a => a.LastName)
                    .ThenBy(a => a.FirstName)
                    .ProjectTo<AuthorDto>(_mapper.ConfigurationProvider)
                    .ToListAsync();

                // For list view, we want empty books for performance
                foreach (var author in authors)
                {
                    author.Books = new List<BookSummaryDto>();
                }

                return Ok(authors);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // GET: api/Author/{id} - Retrieves a specific author by ID with their books
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDto>> GetAuthor(int id)
        {
            try
            {
                // Find author with related books
                var author = await _context.Authors
                    .Include(a => a.AuthorBooks)
                    .ThenInclude(ab => ab.Book)
                    .ThenInclude(b => b.Genre)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    return NotFound();
                }

                // Use AutoMapper to map to DTO
                var authorDto = _mapper.Map<AuthorDto>(author);

                return Ok(authorDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // GET: api/Author/search?query=term - Search authors by first name, last name, or biography
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> SearchAuthors([FromQuery] string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                {
                    return BadRequest("Search query cannot be empty.");
                }

                var lowerQuery = query.ToLower();

                // Search authors with AutoMapper projection
                var results = await _context.Authors
                     .Where(a => a.FirstName.ToLower().Contains(lowerQuery) ||
                                 a.LastName.ToLower().Contains(lowerQuery) ||
                                 (a.Biography != null && a.Biography.ToLower().Contains(lowerQuery)))
                     .OrderBy(a => a.LastName)
                     .ProjectTo<AuthorDto>(_mapper.ConfigurationProvider)
                     .ToListAsync();

                // For search results, we want empty books for performance
                foreach (var author in results)
                {
                    author.Books = new List<BookSummaryDto>();
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region CREATE Operations

        // POST: api/Author - Creates a new author
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> CreateAuthor(CreateAuthorDto createAuthorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Use AutoMapper to create new Author entity from DTO
                var author = _mapper.Map<Author>(createAuthorDto);

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();

                // Use AutoMapper to map to DTO for response
                var authorDto = _mapper.Map<AuthorDto>(author);

                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, authorDto);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error while creating author.");
            }
        }

        #endregion

        #region UPDATE Operations

        // PUT: api/Author/{id} - Updates an existing author
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, UpdateAuthorDto updateAuthorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }

                // Use AutoMapper to update properties from DTO
                _mapper.Map(updateAuthorDto, author);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error while updating author.");
            }
        }

        #endregion

        #region DELETE Operations

        // DELETE: api/Author/{id} - Deletes an author (with business rule checks)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                // Find author with related AuthorBooks
                var author = await _context.Authors
                    .Include(a => a.AuthorBooks)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (author == null)
                {
                    return NotFound();
                }

                // Check if author has any books
                if (author.AuthorBooks.Any())
                {
                    return BadRequest("Cannot delete author with existing books. Remove book associations first.");
                }

                // Safe to delete author
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error while deleting author.");
            }

        }

        #endregion

        #region Helper Methods

        // Checks if an author exists in the database
        private async Task<bool> AuthorExists(int id)
        {
            return await _context.Authors.AnyAsync(a => a.Id == id);
        }

        #endregion
    }
}

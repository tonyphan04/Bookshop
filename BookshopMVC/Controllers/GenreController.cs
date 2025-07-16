using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;

namespace BookshopMVC.Controllers
{
    /// <summary>
    /// Controller for managing genre CRUD operations.
    /// Handles API endpoints for genre management.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GenreController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region READ Operations

        /// <summary>
        /// GET: api/Genre
        /// Retrieves all genres
        /// </summary>
        /// <returns>List of GenreDto</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreDto>>> GetGenres()
        {
            // TODO: Write LINQ to:
            // 1. Query all genres from _context.Genres
            // 2. Order by Name
            // 3. Map to GenreDto (use Select)
            // 4. Include book count for each genre
            // 5. Return Ok(genres)
            // 6. Add try-catch with StatusCode(500) for errors

            throw new NotImplementedException("Write your LINQ logic here!");
        }

        /// <summary>
        /// GET: api/Genre/5
        /// Retrieves a specific genre by ID with its books
        /// </summary>
        /// <param name="id">Genre ID</param>
        /// <returns>GenreDto with book information</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDto>> GetGenre(int id)
        {
            // TODO: Write LINQ to:
            // 1. Find genre by ID using FirstOrDefaultAsync
            // 2. Include related Books
            // 3. Check if genre == null, return NotFound
            // 4. Map to GenreDto with book summaries
            // 5. Return Ok(genreDto)
            // 6. Add try-catch with StatusCode(500) for errors

            throw new NotImplementedException("Write your LINQ logic here!");
        }

        /// <summary>
        /// GET: api/Genre/search?query=fiction
        /// Search genres by name or description
        /// </summary>
        /// <param name="query">Search query string</param>
        /// <returns>List of GenreDto matching the search</returns>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<GenreDto>>> SearchGenres([FromQuery] string query)
        {
            // TODO: Write LINQ to:
            // 1. Validate query is not null/empty, return BadRequest if invalid
            // 2. Convert query to lowercase for case-insensitive search
            // 3. Query genres where Name or Description contains search term
            // 4. Use ToLower().Contains() for case-insensitive matching
            // 5. Order results by Name
            // 6. Map to GenreDto
            // 7. Return Ok(results)
            // 8. Add try-catch with StatusCode(500) for errors

            throw new NotImplementedException("Write your LINQ logic here!");
        }

        #endregion

        #region CREATE Operations

        /// <summary>
        /// POST: api/Genre
        /// Creates a new genre
        /// </summary>
        /// <param name="createGenreDto">Genre creation data</param>
        /// <returns>Created GenreDto</returns>
        [HttpPost]
        public async Task<ActionResult<GenreDto>> CreateGenre(CreateGenreDto createGenreDto)
        {
            // TODO: Write logic to:
            // 1. Validate ModelState.IsValid, return BadRequest(ModelState) if invalid
            // 2. Check if genre with same name already exists
            // 3. If exists, return BadRequest("Genre already exists")
            // 4. Create new Genre entity from DTO
            // 5. Set CreatedDate = DateTime.UtcNow
            // 6. Add to _context.Genres
            // 7. SaveChangesAsync()
            // 8. Map created genre to GenreDto
            // 9. Return CreatedAtAction(nameof(GetGenre), new { id = genre.Id }, genreDto)
            // 10. Add try-catch with StatusCode(500) for errors

            throw new NotImplementedException("Write your logic here!");
        }

        #endregion

        #region UPDATE Operations

        /// <summary>
        /// PUT: api/Genre/5
        /// Updates an existing genre
        /// </summary>
        /// <param name="id">Genre ID to update</param>
        /// <param name="updateGenreDto">Updated genre data</param>
        /// <returns>NoContent if successful</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, UpdateGenreDto updateGenreDto)
        {
            // TODO: Write logic to:
            // 1. Validate ModelState.IsValid, return BadRequest(ModelState) if invalid
            // 2. Find genre by ID using FindAsync
            // 3. Check if genre == null, return NotFound
            // 4. Check if new name conflicts with existing genre (except current one)
            // 5. Update genre properties from DTO
            // 6. SaveChangesAsync()
            // 7. Return NoContent()
            // 8. Add try-catch with StatusCode(500) for errors

            throw new NotImplementedException("Write your logic here!");
        }

        #endregion

        #region DELETE Operations

        /// <summary>
        /// DELETE: api/Genre/5
        /// Deletes a genre (with business rule checks)
        /// </summary>
        /// <param name="id">Genre ID to delete</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            // TODO: Write logic to:
            // 1. Find genre by ID, include Books relationships
            // 2. Check if genre == null, return NotFound
            // 3. Check if genre has any books (Books.Any())
            // 4. If has books, return BadRequest("Cannot delete genre with existing books")
            // 5. Remove the genre
            // 6. SaveChangesAsync()
            // 7. Return NoContent()
            // 8. Add try-catch with StatusCode(500) for errors

            throw new NotImplementedException("Write your logic here!");
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if a genre exists in the database
        /// </summary>
        /// <param name="id">Genre ID</param>
        /// <returns>True if genre exists</returns>
        private async Task<bool> GenreExists(int id)
        {
            // TODO: Write LINQ to:
            // 1. Use _context.Genres.AnyAsync(g => g.Id == id)
            // 2. Return the result

            throw new NotImplementedException("Write your LINQ logic here!");
        }

        /// <summary>
        /// Checks if a genre name already exists (for create/update validation)
        /// </summary>
        /// <param name="name">Genre name</param>
        /// <param name="excludeId">Genre ID to exclude from search (for updates)</param>
        /// <returns>True if name exists</returns>
        private async Task<bool> GenreNameExists(string name, int? excludeId = null)
        {
            // TODO: Write LINQ to:
            // 1. Query genres where name matches (case-insensitive)
            // 2. If excludeId is provided, exclude that genre from search
            // 3. Use AnyAsync to check existence

            throw new NotImplementedException("Write your LINQ logic here!");
        }

        /// <summary>
        /// Maps Genre entity to GenreDto
        /// </summary>
        /// <param name="genre">Genre entity</param>
        /// <returns>GenreDto</returns>
        private GenreDto MapToGenreDto(Genre genre)
        {
            // TODO: Create and return new GenreDto:
            // 1. Map all basic properties (Id, Name, Description, CreatedDate)
            // 2. Calculate book count from genre.Books
            // 3. Create book summaries if Books are loaded

            throw new NotImplementedException("Write your mapping logic here!");
        }

        #endregion
    }
}

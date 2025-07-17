using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;

namespace BookshopMVC.Controllers
{
    // Controller for managing shopping cart operations and checkout workflow
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region READ Operations

        // GET: api/Cart/user/{userId} - Retrieves the cart for a specific user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CartDto>> GetUserCart(int userId)
        {
            try
            {
                var cartItems = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .Include(c => c.Book)
                    .ThenInclude(b => b.Genre)
                    .Select(c => new CartItemDto
                    {
                        Id = c.Id,
                        UserId = c.UserId,
                        BookId = c.BookId,
                        BookTitle = c.Book.Title,
                        BookImageUrl = c.Book.ImageUrl,
                        BookPrice = c.Book.Price,
                        Quantity = c.Quantity,
                        TotalPrice = c.Book.Price * c.Quantity,
                        AddedDate = c.AddedDate,
                        AvailableStock = c.Book.Stock
                    })
                    .ToListAsync();

                var cartDto = new CartDto
                {
                    UserId = userId,
                    Items = cartItems,
                    LastUpdated = DateTime.UtcNow
                };

                return Ok(cartDto);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        // GET: api/Cart/count/user/{userId} - Gets the count of items in user's cart
        [HttpGet("count/user/{userId}")]
        public async Task<ActionResult<int>> GetCartItemCount(int userId)
        {
            try
            {
                var sum = await _context.CartItems
                    .Where(c => c.UserId == userId)
                    .SumAsync(c => c.Quantity);

                return Ok(sum);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"Internal server error: {e.Message}");
            }
        }

        #endregion

        #region CREATE Operations

        // POST: api/Cart/add - Adds an item to the cart or updates quantity if item already exists
        [HttpPost("add")]
        public async Task<ActionResult<CartItemDto>> AddToCart(AddToCartDto addToCartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if book exists and get its details
                var book = await _context.Books.FindAsync(addToCartDto.BookId);
                if (book == null)
                {
                    return BadRequest("Book not found");
                }

                // Check if book has sufficient stock
                if (book.Stock < addToCartDto.Quantity)
                {
                    return BadRequest($"Insufficient stock. Available: {book.Stock}");
                }

                // Check if user already has this book in cart
                var existingCartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == addToCartDto.UserId && c.BookId == addToCartDto.BookId);

                if (existingCartItem != null)
                {
                    // Update existing cart item
                    var newQuantity = existingCartItem.Quantity + addToCartDto.Quantity;
                    if (newQuantity > book.Stock)
                    {
                        return BadRequest($"Total quantity ({newQuantity}) exceeds available stock ({book.Stock})");
                    }
                    existingCartItem.Quantity = newQuantity;
                }
                else
                {
                    // Create new cart item
                    var newCartItem = new CartItem
                    {
                        UserId = addToCartDto.UserId,
                        BookId = addToCartDto.BookId,
                        Quantity = addToCartDto.Quantity,
                        AddedDate = DateTime.UtcNow
                    };
                    _context.CartItems.Add(newCartItem);
                    existingCartItem = newCartItem; // For mapping below
                }

                await _context.SaveChangesAsync();

                // Load book details for mapping
                await _context.Entry(existingCartItem)
                    .Reference(c => c.Book)
                    .LoadAsync();

                var cartItemDto = MapToCartItemDto(existingCartItem);
                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region UPDATE Operations

        // PUT: api/Cart/update - Updates the quantity of an item in the cart
        [HttpPut("update")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(UpdateCartItemDto updateCartDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Find the cart item by UserId and BookId
                var cartItem = await _context.CartItems
                    .Include(c => c.Book)
                    .FirstOrDefaultAsync(c => c.UserId == updateCartDto.UserId && c.BookId == updateCartDto.BookId);

                if (cartItem == null)
                {
                    return NotFound("Cart item not found");
                }

                // Check if book has sufficient stock for new quantity
                if (cartItem.Book.Stock < updateCartDto.Quantity)
                {
                    return BadRequest($"Insufficient stock. Available: {cartItem.Book.Stock}");
                }

                // Update the quantity
                cartItem.Quantity = updateCartDto.Quantity;

                await _context.SaveChangesAsync();

                // Map to CartItemDto and return
                var cartItemDto = MapToCartItemDto(cartItem);
                return Ok(cartItemDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region DELETE Operations

        // DELETE: api/Cart/remove/user/{userId}/book/{bookId} - Removes a specific item from the cart
        [HttpDelete("remove/user/{userId}/book/{bookId}")]
        public async Task<IActionResult> RemoveFromCart(int userId, int bookId)
        {
            try
            {
                // Find the specific cart item by UserId and BookId
                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.UserId == userId && c.BookId == bookId);

                if (cartItem == null)
                {
                    return NotFound("Cart item not found");
                }

                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Cart/clear/user/{userId} - Clears all items from user's cart
        [HttpDelete("clear/user/{userId}")]
        public async Task<IActionResult> ClearCart(int userId)
        {
            // TODO: Write logic to:
            // 1. Get all cart items for the user
            // 2. Remove all items using RemoveRange
            // 3. SaveChangesAsync()
            // 4. Return NoContent()
            // 5. Add try-catch with StatusCode(500) for errors

            try
            {
                // Get all cart items for the user
                var cartItems = await _context.CartItems
                    .Where(a => a.UserId == userId)
                    .ToListAsync();

                if (cartItems.Any())
                {
                    _context.CartItems.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        // Checks if a cart item exists for a user and book
        private async Task<bool> CartItemExists(int userId, int bookId)
        {
            try
            {
                var cartItemExist = await _context.CartItems.AnyAsync(c => c.UserId == userId && c.BookId == bookId);
                return cartItemExist;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        // Gets available stock for a book
        private async Task<int> GetAvailableStock(int bookId)
        {
            try
            {
                var book = await _context.Books
                    .Where(a => a.Id == bookId)
                    .FirstOrDefaultAsync();

                if (book == null)
                {
                    return 0;
                }
                return book.Stock;
            }
            catch (Exception)
            {
                return 0; // Return 0 if error occurs
            }
        }

        // Maps CartItem entity to CartItemDto
        private CartItemDto MapToCartItemDto(CartItem cartItem)
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
                TotalPrice = (cartItem.Book?.Price ?? 0) * cartItem.Quantity,
                AddedDate = cartItem.AddedDate,
                AvailableStock = cartItem.Book?.Stock ?? 0
            };
        }

        #endregion
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;

namespace BookshopMVC.Controllers
{
    /// <summary>
    /// Controller for managing shopping cart operations.
    /// Handles API endpoints for cart management (add, remove, update quantities, view cart).
    /// </summary>
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

        /// <summary>
        /// GET: api/Cart/user/5
        /// Retrieves the cart for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>CartDto with all cart items</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<CartDto>> GetUserCart(int userId)
        {
            // TODO: Write LINQ to:
            // 1. Query all cart items for the user from _context.CartItems
            // 2. Include Book and Book.Genre information
            // 3. Map to CartDto with CartItemDto list
            // 4. Calculate total price and total items
            // 5. Return Ok(cartDto)
            // 6. Add try-catch with StatusCode(500) for errors

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

        /// <summary>
        /// GET: api/Cart/count/user/5
        /// Gets the count of items in user's cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Number of items in cart</returns>
        [HttpGet("count/user/{userId}")]
        public async Task<ActionResult<int>> GetCartItemCount(int userId)
        {
            // TODO: Write LINQ to:
            // 1. Sum the quantities of all cart items for the user
            // 2. Use _context.CartItems.Where(c => c.UserId == userId).SumAsync(c => c.Quantity)
            // 3. Return Ok(count)
            // 4. Add try-catch with StatusCode(500) for errors

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

        /// <summary>
        /// POST: api/Cart/add
        /// Adds an item to the cart or updates quantity if item already exists
        /// </summary>
        /// <param name="addToCartDto">Cart item data</param>
        /// <returns>Updated CartItemDto</returns>
        [HttpPost("add")]
        public async Task<ActionResult<CartItemDto>> AddToCart(AddToCartDto addToCartDto)
        {
            // TODO: Write logic to:
            // 1. Validate ModelState.IsValid, return BadRequest(ModelState) if invalid
            // 2. Check if book exists and has sufficient stock
            // 3. Check if user already has this book in cart
            // 4. If exists, update quantity (ensure total doesn't exceed stock)
            // 5. If not exists, create new CartItem
            // 6. SaveChangesAsync()
            // 7. Map to CartItemDto and return Ok(cartItemDto)
            // 8. Add try-catch with StatusCode(500) for errors

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

        /// <summary>
        /// PUT: api/Cart/update
        /// Updates the quantity of an item in the cart
        /// </summary>
        /// <param name="updateCartDto">Updated cart item data</param>
        /// <returns>Updated CartItemDto</returns>
        [HttpPut("update")]
        public async Task<ActionResult<CartItemDto>> UpdateCartItem(UpdateCartItemDto updateCartDto)
        {
            // TODO: Write logic to:
            // 1. Validate ModelState.IsValid, return BadRequest(ModelState) if invalid
            // 2. Find the cart item by UserId and BookId
            // 3. Check if cart item exists, return NotFound if not
            // 4. Check if book has sufficient stock for new quantity
            // 5. Update the quantity
            // 6. SaveChangesAsync()
            // 7. Map to CartItemDto and return Ok(cartItemDto)
            // 8. Add try-catch with StatusCode(500) for errors

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

        /// <summary>
        /// DELETE: api/Cart/remove/user/5/book/10
        /// Removes a specific item from the cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="bookId">Book ID</param>
        /// <returns>NoContent if successful</returns>
        [HttpDelete("remove/user/{userId}/book/{bookId}")]
        public async Task<IActionResult> RemoveFromCart(int userId, int bookId)
        {
            // TODO: Write logic to:
            // 1. Find the cart item by UserId and BookId
            // 2. Check if cart item exists, return NotFound if not
            // 3. Remove the cart item from _context.CartItems
            // 4. SaveChangesAsync()
            // 5. Return NoContent()
            // 6. Add try-catch with StatusCode(500) for errors

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

        /// <summary>
        /// DELETE: api/Cart/clear/user/5
        /// Clears all items from user's cart
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>NoContent if successful</returns>
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

        /// <summary>
        /// Checks if a cart item exists for a user and book
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="bookId">Book ID</param>
        /// <returns>True if cart item exists</returns>
        private async Task<bool> CartItemExists(int userId, int bookId)
        {
            // TODO: Write LINQ to:
            // 1. Use _context.CartItems.AnyAsync(c => c.UserId == userId && c.BookId == bookId)
            // 2. Return the result

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

        /// <summary>
        /// Gets available stock for a book
        /// </summary>
        /// <param name="bookId">Book ID</param>
        /// <returns>Available stock quantity</returns>
        private async Task<int> GetAvailableStock(int bookId)
        {
            // TODO: Write LINQ to:
            // 1. Find the book by ID
            // 2. Return book.Stock or 0 if book not found

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

        /// <summary>
        /// Maps CartItem entity to CartItemDto
        /// </summary>
        /// <param name="cartItem">CartItem entity</param>
        /// <returns>CartItemDto</returns>
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

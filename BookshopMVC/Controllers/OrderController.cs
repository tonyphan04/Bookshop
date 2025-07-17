using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;

namespace BookshopMVC.Controllers
{
    // Controller for managing order operations and checkout workflow
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region READ Operations

        // GET: api/Order - Retrieves all orders (admin view)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            try
            {
                // ✅ Query all orders with related data (admin view)
                var orders = await _context.Orders
                    .Include(o => o.User)        // Include customer information
                    .Include(o => o.Items)       // ✅ FIXED: Use correct property name 'Items'
                        .ThenInclude(oi => oi.Book) // Include book details for each item
                    .OrderByDescending(o => o.OrderDate) // Newest orders first
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        UserName = o.User.FirstName + " " + o.User.LastName, // ✅ FIXED: Use UserName
                        OrderDate = o.OrderDate,
                        TotalPrice = o.TotalPrice,   // ✅ FIXED: Use correct property name
                        Status = o.Status.ToString(), // ✅ FIXED: Convert enum to string
                        CreatedAt = o.CreatedDate,   // ✅ FIXED: Use correct property name
                        ItemCount = o.Items.Sum(oi => oi.Quantity) // ✅ FIXED: Use ItemCount
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex) // ✅ FIXED: Consistent error handling
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Order/5 - Retrieves a specific order by ID with full details
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            try
            {
                // ✅ Find order by ID with related data included
                var order = await _context.Orders
                    .Include(o => o.User)        // Include customer information
                    .Include(o => o.Items)       // Include order items
                        .ThenInclude(oi => oi.Book) // Include book details for each item
                    .FirstOrDefaultAsync(o => o.Id == id); // ✅ FIXED: Proper lambda expression

                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                // ✅ Map to OrderWithItemsDto for detailed view
                var orderDto = new OrderWithItemsDto
                {
                    Id = order.Id,
                    UserId = order.UserId,
                    UserName = order.User.FirstName + " " + order.User.LastName,
                    OrderDate = order.OrderDate,
                    TotalPrice = order.TotalPrice,
                    Status = order.Status.ToString(),
                    CreatedAt = order.CreatedDate,
                    // Map order items with full details
                    OrderItems = order.Items.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        OrderId = oi.OrderId,
                        BookId = oi.BookId,
                        BookTitle = oi.Book.Title,
                        UnitPrice = oi.UnitPrice,
                        Quantity = oi.Quantity
                    }).ToList()
                };

                return Ok(orderDto);
            }
            catch (Exception ex) // ✅ FIXED: Consistent error handling
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Order/user/5 - Retrieves all orders for a specific user
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetUserOrders(int userId)
        {
            try
            {
                // ✅ Query orders for specific user with related data
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId) // Filter by user ID
                    .Include(o => o.User)           // Include user info
                    .Include(o => o.Items)          // Include order items
                        .ThenInclude(oi => oi.Book) // Include book details
                    .OrderByDescending(o => o.OrderDate) // Newest first
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        UserName = o.User.FirstName + " " + o.User.LastName,
                        OrderDate = o.OrderDate,
                        TotalPrice = o.TotalPrice,
                        Status = o.Status.ToString(),
                        CreatedAt = o.CreatedDate,
                        ItemCount = o.Items.Sum(oi => oi.Quantity)
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Order/status/Pending - Retrieves orders by status (admin functionality)
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByStatus(OrderStatus status)
        {
            try
            {
                // ✅ Query orders by status with related data
                var orders = await _context.Orders
                    .Where(o => o.Status == status) // Filter by status
                    .Include(o => o.User)           // Include user info
                    .Include(o => o.Items)          // Include order items
                        .ThenInclude(oi => oi.Book) // Include book details
                    .OrderByDescending(o => o.OrderDate) // Newest first
                    .Select(o => new OrderDto
                    {
                        Id = o.Id,
                        UserId = o.UserId,
                        UserName = o.User.FirstName + " " + o.User.LastName,
                        OrderDate = o.OrderDate,
                        TotalPrice = o.TotalPrice,
                        Status = o.Status.ToString(),
                        CreatedAt = o.CreatedDate,
                        ItemCount = o.Items.Sum(oi => oi.Quantity)
                    })
                    .ToListAsync();

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region CREATE Operations

        // POST: api/Order/checkout - Creates an order from user's cart items
        [HttpPost("checkout")]
        public async Task<ActionResult<OrderDto>> CheckoutCart(CheckoutDto checkoutDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // ✅ Get all cart items for the user with book details
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Book)
                    .Where(ci => ci.UserId == checkoutDto.UserId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return BadRequest("Cart is empty. Add items before checkout.");
                }

                // ✅ Check stock availability for all items
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.Book.Stock < cartItem.Quantity)
                    {
                        return BadRequest($"Insufficient stock for '{cartItem.Book.Title}'. Available: {cartItem.Book.Stock}, Requested: {cartItem.Quantity}");
                    }
                }

                // ✅ Create new Order entity
                var order = new Order
                {
                    UserId = checkoutDto.UserId,
                    OrderDate = DateTime.UtcNow,
                    TotalPrice = CalculateTotalPrice(cartItems),
                    Status = OrderStatus.Pending,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // Save to get order ID

                // ✅ Create OrderItems from CartItems
                var orderItems = cartItems.Select(ci => new OrderItem
                {
                    OrderId = order.Id,
                    BookId = ci.BookId,
                    UnitPrice = ci.Book.Price,
                    Quantity = ci.Quantity
                }).ToList();

                _context.OrderItems.AddRange(orderItems); // ✅ FIXED: Use AddRange for collections

                // ✅ Update book stock quantities
                foreach (var cartItem in cartItems)
                {
                    cartItem.Book.Stock -= cartItem.Quantity;
                }

                // ✅ Clear user's cart
                _context.CartItems.RemoveRange(cartItems);

                // Save all changes
                await _context.SaveChangesAsync();

                // ✅ Map to OrderDto for response
                var orderDto = MapToOrderDto(order);

                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, orderDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region UPDATE Operations

        // PUT: api/Order/5/status - Updates the status of an order (admin functionality)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, UpdateOrderStatusDto updateStatusDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // ✅ Find order by ID
                var order = await _context.Orders.FindAsync(id);
                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                // ✅ Validate status transition using business rules
                if (!IsValidStatusTransition(order.Status, updateStatusDto.Status))
                {
                    return BadRequest($"Invalid status transition from {order.Status} to {updateStatusDto.Status}.");
                }

                // ✅ Update order status
                order.Status = updateStatusDto.Status;

                // Save changes to database
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region DELETE Operations

        // DELETE: api/Order/5 - Cancels an order (only if status is Pending)
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                // ✅ Find order by ID with order items and books
                var order = await _context.Orders
                    .Include(o => o.Items)
                        .ThenInclude(oi => oi.Book)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (order == null)
                {
                    return NotFound($"Order with ID {id} not found.");
                }

                // ✅ Check if order can be cancelled (only Pending orders can be cancelled)
                if (order.Status != OrderStatus.Pending)
                {
                    return BadRequest($"Cannot cancel order with status '{order.Status}'. Only pending orders can be cancelled.");
                }

                // ✅ Restore book stock from order items
                foreach (var orderItem in order.Items)
                {
                    orderItem.Book.Stock += orderItem.Quantity;
                }

                // ✅ Update order status to Cancelled
                order.Status = OrderStatus.Cancelled;

                // Save all changes
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        #endregion

        #region Helper Methods

        // Checks if an order exists in the database
        private async Task<bool> OrderExists(int id)
        {
            // ✅ Use AnyAsync to check if order exists by ID
            return await _context.Orders.AnyAsync(o => o.Id == id);
        }

        // Validates if order status transition is allowed
        private bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
        {
            // ✅ Implement business logic for valid status transitions
            return currentStatus switch
            {
                OrderStatus.Pending => newStatus == OrderStatus.Confirmed || newStatus == OrderStatus.Cancelled,
                OrderStatus.Confirmed => newStatus == OrderStatus.Completed || newStatus == OrderStatus.Cancelled,
                OrderStatus.Completed => false, // No changes allowed once completed
                OrderStatus.Cancelled => false, // No changes allowed once cancelled
                _ => false
            };
        }

        // Calculates total price for cart items
        private decimal CalculateTotalPrice(List<CartItem> cartItems)
        {
            // ✅ Calculate total price using the TotalPrice computed property
            return cartItems.Sum(item => item.TotalPrice);
        }

        // Maps Order entity to OrderDto
        private OrderDto MapToOrderDto(Order order)
        {
            // ✅ Create and return new OrderDto with all properties mapped
            return new OrderDto
            {
                Id = order.Id,
                UserId = order.UserId,
                UserName = order.User?.FirstName + " " + order.User?.LastName,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status.ToString(),
                CreatedAt = order.CreatedDate,
                ItemCount = order.Items?.Sum(oi => oi.Quantity) ?? 0
            };
        }

        #endregion
    }
}

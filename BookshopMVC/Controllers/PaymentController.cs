using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookshopMVC.DTOs;
using BookshopMVC.Data;
using BookshopMVC.Models;
using Stripe;

namespace BookshopMVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PaymentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            // Stripe is now configured in Program.cs
        }

        // Create payment intent for order
        [HttpPost("create/{orderId}")]
        public async Task<IActionResult> CreatePayment(int orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order == null)
                return NotFound("Order not found");

            if (order.Status != OrderStatus.Pending)
                return BadRequest("Order is not pending payment");

            try
            {
                var service = new PaymentIntentService();
                var intent = await service.CreateAsync(new PaymentIntentCreateOptions
                {
                    Amount = (long)(order.TotalPrice * 100), // Convert to cents
                    Currency = "usd",
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string> { ["order_id"] = orderId.ToString() }
                });

                // Save payment record
                var payment = new Payment
                {
                    OrderId = orderId,
                    Amount = order.TotalPrice,
                    PaymentMethod = Models.PaymentMethod.CreditCard,
                    Status = PaymentStatus.Pending,
                    StripePaymentIntentId = intent.Id,
                    StripeClientSecret = intent.ClientSecret,
                    CreatedDate = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Ok(new { clientSecret = intent.ClientSecret });
            }
            catch (StripeException ex)
            {
                return BadRequest($"Payment creation failed: {ex.Message}");
            }
        }

        // Complete payment after successful Stripe payment
        [HttpPost("complete/{orderId}")]
        public async Task<IActionResult> CompletePayment(int orderId)
        {
            var payment = await _context.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
                return NotFound("Payment not found");

            try
            {
                var service = new PaymentIntentService();
                var intent = await service.GetAsync(payment.StripePaymentIntentId);

                if (intent.Status == "succeeded")
                {
                    payment.Status = PaymentStatus.Succeeded;
                    payment.CompletedDate = DateTime.UtcNow;
                    payment.Order.Status = OrderStatus.Confirmed;

                    await _context.SaveChangesAsync();
                    return Ok("Payment completed successfully");
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                    await _context.SaveChangesAsync();
                    return BadRequest("Payment failed");
                }
            }
            catch (StripeException ex)
            {
                return BadRequest($"Payment verification failed: {ex.Message}");
            }
        }

        // Get payment status for order
        [HttpGet("status/{orderId}")]
        public async Task<IActionResult> GetPaymentStatus(int orderId)
        {
            var payment = await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);

            if (payment == null)
                return NotFound("No payment found");

            return Ok(new
            {
                status = payment.Status.ToString(),
                amount = payment.Amount,
                createdDate = payment.CreatedDate,
                completedDate = payment.CompletedDate
            });
        }
    }
}

# Cart to Order Conversion Flow

## Overview
This document explains the complete business logic for converting CartItems to OrderItems during the checkout process.

## The Pattern

### CartItem (Temporary & Mutable)
- **Purpose**: Store temporary shopping selections
- **Lifetime**: Until checkout or cart abandonment
- **Mutability**: Can be updated, removed, quantity changed
- **Storage**: Database table for persistence across sessions

### OrderItem (Permanent & Immutable)
- **Purpose**: Historical record of actual purchases
- **Lifetime**: Permanent business record
- **Mutability**: Read-only after creation
- **Storage**: Database table as legal/financial record

## Code Example: Checkout Process

```csharp
// Example service method for checkout
public async Task<Order> CheckoutAsync(int userId)
{
    // 1. Get all cart items for the user
    var cartItems = await _context.CartItems
        .Include(ci => ci.Book)
        .Where(ci => ci.UserId == userId)
        .ToListAsync();

    if (!cartItems.Any())
        throw new InvalidOperationException("Cart is empty");

    // 2. Create new order
    var order = new Order
    {
        UserId = userId,
        OrderDate = DateTime.UtcNow,
        Status = OrderStatus.Pending
    };

    _context.Orders.Add(order);
    await _context.SaveChangesAsync(); // Save to get OrderId

    // 3. Convert CartItems to OrderItems using our mapping extension
    var orderItems = cartItems.ToOrderItems(order.Id);
    
    // 4. Add OrderItems to database
    _context.OrderItems.AddRange(orderItems);

    // 5. Clear the cart (CartItems are temporary)
    _context.CartItems.RemoveRange(cartItems);

    // 6. Update order total
    order.Total = orderItems.Sum(oi => oi.TotalPrice);

    await _context.SaveChangesAsync();

    return order;
}
```

## Key Business Rules

### Price Capture
```csharp
UnitPrice = cartItem.Book?.Price ?? 0 // Captures price AT TIME OF ORDER
```
- **Why**: Prices may change after order is placed
- **Result**: Order reflects actual price paid, not current price

### Quantity Preservation
```csharp
Quantity = cartItem.Quantity // Exact quantity from cart
```
- **Why**: User's intended purchase amount
- **Result**: Order reflects what user actually ordered

### Immutability
- Once `OrderItem` is created, it cannot be modified
- Represents legal/financial commitment
- Historical accuracy for business records

## Data Flow Diagram

```
User Shopping:
CartItem → [Add/Update/Remove] → CartItem (modified)

Checkout Process:
CartItem → [ToOrderItem()] → OrderItem (permanent)
CartItem → [Delete] → ∅ (cart cleared)

Post-Order:
OrderItem → [Read-only] → Order History
```

## Database State Changes

### Before Checkout
```
CartItems Table:
- UserId: 123, BookId: 1, Quantity: 2
- UserId: 123, BookId: 5, Quantity: 1

Orders Table: (empty for this user)
OrderItems Table: (empty for this user)
```

### After Checkout
```
CartItems Table: (cleared)

Orders Table:
- Id: 1, UserId: 123, OrderDate: 2024-01-15, Total: 45.97

OrderItems Table:
- Id: 1, OrderId: 1, BookId: 1, Quantity: 2, UnitPrice: 19.99
- Id: 2, OrderId: 1, BookId: 5, Quantity: 1, UnitPrice: 5.99
```

## Benefits of This Pattern

1. **Clear Separation**: Cart vs Order logic are distinct
2. **Data Integrity**: Orders are immutable historical records
3. **Performance**: Cart can be modified without affecting orders
4. **Business Logic**: Price capture, quantity preservation
5. **User Experience**: Cart changes don't affect past orders

## Usage in Controllers

```csharp
[HttpPost("checkout")]
public async Task<IActionResult> Checkout()
{
    var userId = GetCurrentUserId();
    
    try
    {
        var order = await _orderService.CheckoutAsync(userId);
        return Ok(order.ToDto());
    }
    catch (InvalidOperationException ex)
    {
        return BadRequest(ex.Message);
    }
}
```

This pattern ensures clean separation between mutable shopping cart functionality and immutable order history, following e-commerce best practices.

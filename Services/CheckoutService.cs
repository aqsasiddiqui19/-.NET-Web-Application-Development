using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Ecommerce_API_Project.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;


public class CheckoutService

{ private readonly IHttpContextAccessor _httpContextAccessor;
    private const string CartSessionKeyPrefix = "Cart_";
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;

    public CheckoutService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, CartService cartService)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _cartService = cartService; 
    }

    public (Order Order, Checkout Checkout) PlaceGuestOrder(CheckoutViewModel model)
    {
        // Retrieve the cart ID for the current session/user
        var cartId = _cartService.GetOrCreateCartId();

        // Retrieve the cart items for the given cart ID
        var items = _cartService.GetCartItems(cartId);

        // Validate CartId - ensure it is not empty or invalid
        if (cartId == Guid.Empty)
        {
            throw new ArgumentException("Invalid CartId.");
        }

        // Validate the cart has items before proceeding with checkout
        if (items == null || !items.Any())
        {
            throw new ArgumentException("The cart is empty.");
        }

        if (string.IsNullOrEmpty(model.Email))
            throw new ArgumentException("Email is required.");

        // Check if user exists, if not create one
        var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
        if (user == null)
        {
            user = new User { Email = model.Email };
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        // Manual validation
        if (string.IsNullOrWhiteSpace(model.FirstName) || model.FirstName.Trim().ToLower() == "string")
            throw new ArgumentException("First Name is required.");
        if (string.IsNullOrWhiteSpace(model.LastName)|| model.LastName.Trim().ToLower() == "string")
            throw new ArgumentException("Last Name is required.");
        if (string.IsNullOrWhiteSpace(model.AddressLine1) || model.AddressLine1.Trim().ToLower() == "string")
            throw new ArgumentException("Address Line 1 is required.");
        if (string.IsNullOrWhiteSpace(model.AddressLine2) || model.AddressLine2.Trim().ToLower() == "string")
            throw new ArgumentException("Address Line 1 is required.");
        if (string.IsNullOrWhiteSpace(model.City)|| model.City.Trim().ToLower() == "string")
            throw new ArgumentException("City is required.");
        if (string.IsNullOrWhiteSpace(model.PostalCode) || model.PostalCode.Trim().ToLower() == "string")
            throw new ArgumentException("Postal Code is required.");
        if (string.IsNullOrWhiteSpace(model.PhoneNumber)|| model.PhoneNumber.Trim().ToLower() == "string")
            throw new ArgumentException("Phone Number is required.");
        if (string.IsNullOrWhiteSpace(model.Province)|| model.Province.Trim().ToLower() == "string")
            throw new ArgumentException("Province is required.");

        // Save shipping info
        var shipping = new Shipping
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            AddressLine1 = model.AddressLine1,
            AddressLine2 = model.AddressLine2,
            City = model.City,
            PostalCode = model.PostalCode,
            PhoneNumber = model.PhoneNumber,
            Province=model.Province,
            UserId = user.UserId
        };
        _context.Shippings.Add(shipping);
        _context.SaveChanges();

        // Calculate total price and total items
        decimal totalPrice = items.Sum(i => i.TotalPrice);
        int totalItems = items.Sum(i => i.Quantity);

        // Create checkout before order
        var checkout = new Checkout
        {
            CartId = cartId,
            Email = model.Email,
            TotalPrice = totalPrice,
            TotalItems = totalItems,
            ShippingId = shipping.ShippingId
        };

        // Create order
        var order = new Order
        {
            UserId = user.UserId,
            CartId = cartId,
            Email = model.Email,
            TotalAmount = totalPrice, // Fixed: was totalAmount (undefined)
            TotalItems = totalItems,
            OrderStatus = "Pending",
            OrderDate = DateTime.UtcNow,
            ShippingId = shipping.ShippingId
        };
        _context.Orders.Add(order);
        _context.SaveChanges();

        // Add order items and update product stock
        foreach (var item in items)
        {
            var orderItem = new OrderItem
            {
                OrderId = order.OrderId,
                ProductId = item.ProductId,
                ProductName= item.ProductName,  
                Quantity = item.Quantity,
                FinalAmount = item.FinalAmount,
                TotalPrice = item.TotalPrice,
            };
            _context.OrderItems.Add(orderItem);

            var cartItemEntity = new CartItem
            { 
                CartId = cartId,
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                FinalAmount = item.FinalAmount,
                TotalPrice = item.TotalPrice
            };
            _context.CartItems.Add(cartItemEntity);

            var cart = new Cart
            {
                CartId = cartId,
                UserId = user.UserId,
                TotalPrice= totalPrice,
                TotalItems= totalItems,
                CreatedAt = DateTime.UtcNow,

            };
            _context.Carts.Add(cart);

            // Reduce stock
            var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
            if (product != null)
            {
                if (product.ProductStock >= item.Quantity)
                {
                    product.ProductStock -= item.Quantity;
                }
                else
                {
                    throw new InvalidOperationException($"Insufficient stock for product '{product.ProductName}'.");
                }
          }
        }

        _context.Checkouts.Add(checkout);
        _context.SaveChanges();
        // Clear the cart after placing the order
        _cartService.ClearCart(cartId);

        return (order, checkout);
    }
}

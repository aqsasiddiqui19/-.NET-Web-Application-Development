using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Services;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_API_Project.Models;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;
    private readonly CheckoutService _checkoutService;

    // Constructor to inject the required services (Database context, CartService, CheckoutService)
    public CheckoutController(ApplicationDbContext context, CartService cartService, CheckoutService checkoutService)
    {
        _context = context;
        _cartService = cartService;
        _checkoutService = checkoutService;
    }

    [HttpPost("guest-checkout")]
    public IActionResult GuestCheckout([FromForm] CheckoutViewModel model)
    {
        try
        {
            // Call the CheckoutService to handle order placement logic
            var result = _checkoutService.PlaceGuestOrder(model);
           
            // Return success response with OrderId and CheckoutId
            return Ok(new
            {
                OrderId = result.Order.OrderId,
                CheckoutId = result.Checkout.CheckoutId,
                Message = "Order placed successfully."
            });
        }
        catch (DbUpdateException dbEx)
        {
            // Handle database-specific exceptions (e.g., saving to DB fails)
            var innerMessage = dbEx.InnerException?.Message ?? dbEx.Message;
            return BadRequest(new
            {
                Message = "Database update failed.",
                Details = innerMessage
            });
        }
        catch (Exception ex)
        {
            // Handle all other exceptions (e.g., unexpected errors)
            return BadRequest(new
            {
                Message = "An error occurred.",
                Details = ex.Message
            });
        }
    }
}



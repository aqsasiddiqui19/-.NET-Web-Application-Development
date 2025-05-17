using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Ecommerce_API_Project.Services;
using Microsoft.IdentityModel.Tokens;

namespace CartItemController.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly CartService _cartService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    // Initializes a new instance of the CartController class.
    // This constructor is used to inject dependencies such as the database context,cartService, and httpContextAccessor
    //context:The ApplicationDbContext instance used to interact with the database.</param>
    // cartService: The CartService instance that provides methods for managing the shopping cart.
    //httpContextAccessor:The IHttpContextAccessor instance used to retrieve HTTP context information, such as session data.
    public CartController(ApplicationDbContext context, CartService cartService, IHttpContextAccessor httpContextAccessor)
    {
        // Assign the injected ApplicationDbContext to the local context variable
        _context = context;
        
        // Assign the injected CartService to the local cartService variable
        _cartService=cartService;
 
        // Assign the injected IHttpContextAccessor to the local httpContextAccessor variable
        _httpContextAccessor=httpContextAccessor;
    }

    // POST:api/Cart/add-to-cart
    [HttpPost("add-to-cart")]

    // Adds a product to the user's shopping cart.
    public IActionResult AddToCart([FromForm] CartItemViewModel cartItem)
    {
        // Validate the model state to ensure the request data is valid
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Retrieve product details from the database
        var product = _context.Products.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
        if (product == null)
        {
            return BadRequest("ProductId not found.");
        }

        // Check if the product exists in the database
        var productName = _context.Products.FirstOrDefault(p => p.ProductName == cartItem.ProductName);
        if (productName == null)
        {
            return BadRequest("Invalid Product Name.");
        }

        // Check if the cart item quantity is at least 1

        if (cartItem.Quantity < 1)
        {
            return BadRequest(new { message = "Quantity must be at least 1." });
        }

        // Check if the cart item quantity exceeds the available product stock

        if (cartItem.Quantity > product.ProductStock)
        {
            return BadRequest(new { message = "Requested quantity exceeds available stock." });
        }

        // Get or create a cart ID for the current user/session
        var cartId = _cartService.GetOrCreateCartId();

        // Validate CartId - ensure it is not empty
        if (cartId == Guid.Empty)
        {
            return BadRequest("Invalid CartId.");
        }

        // Create a new cart item based on the provided cart item data
        var cartItemModel = new CartItem
        {
            CartId = cartId,
            ProductId = cartItem.ProductId,
            Quantity = cartItem.Quantity,
            ProductName = cartItem.ProductName,
            FinalAmount = product.FinalAmount
        };

        // Add item to cart
        _cartService.AddToCart(cartId, cartItemModel);

        // Retrieve updated cart details
        var cart = _cartService.GetCartItems(cartId);
        var totalItems = cart.Sum(c => c.Quantity);
        var totalPrice = cart.Sum(c => c.TotalPrice);

        // Return a success response with updated cart information
        return Ok(new
        {
            message = "Item added to cart.",
            cart,
            TotalItems = totalItems,
            TotalPrice = totalPrice
        });

    }

    // POST:api/Cart/IncrementItemByProductId/{id}
    [HttpPost("increment-item-by-id")]
   
    //Increments the quantity of a specific product in the cart
    public IActionResult IncrementItemByProductId(Guid productId)
    {
        // Get the cart ID for the current session/user
        var cartId = _cartService.GetOrCreateCartId();

        // Validate CartId - ensure it is not empty
        if (cartId == Guid.Empty)
        {
            return BadRequest("Invalid CartId.");
        }

        // Increment the item quantity in the cart
        _cartService.IncrementItem(cartId, productId);

        // Return a success message
        return Ok(new { message = "Item quantity increased." });
    }

    // POST:api/Cart/decrement-item-by-productId/{productId}
    [HttpPost("decrement-item-by-productId")]

    // Decrements the quantity of a specific product in the cart.
    public IActionResult DecrementItemById(Guid productId)
    {
        // Get or create the cart ID for the current session/user
        var cartId = _cartService.GetOrCreateCartId();

        // Validate CartId - ensure it is not empty
        if (cartId == Guid.Empty)
        {
            return BadRequest("Invalid CartId.");
        }

        // Decrement the item quantity in the cart
        _cartService.DecrementItem(cartId, productId);

        // Return a success message
        return Ok(new { message = "Item quantity decreased." });
    }

    // DELETE: api/Cart/remove-item-by-productId/{productId}
    [HttpDelete("remove-item-by-productId")]

    // Removes a specific product from the cart.
    public IActionResult RemoveItemById(Guid productId)
    {
        // Get the cart ID for the current session/user
        var cartId = _cartService.GetOrCreateCartId();

        // Validate CartId - ensure it is not empty
        if (cartId == Guid.Empty)
        {
            return BadRequest("Invalid CartId.");
        }

        // Remove the item from the cart
        _cartService.RemoveItem(cartId, productId);

        // Return a success messages
        return Ok(new { message = "Item removed from cart." });
    }

    // DELETE: api/Cart/Clear
    [HttpDelete("clear")]

    // Clears all items from the user's shopping cart.
    public IActionResult ClearCart()
    {
        // Get the cart ID for the current session/user
        var cartId = _cartService.GetOrCreateCartId();

        // Validate CartId - ensure it is not empty
        if (cartId == Guid.Empty)
        {
            return BadRequest("Invalid CartId.");
        }

        // Clear the cart by removing all items
        _cartService.ClearCart(cartId);

        // Return a success message
        return Ok(new { message = "Cart has been cleared successfully." });
    }

   // GET: api/Cart/View
    [HttpGet("view")]

    // Retrieves the current state of the shopping cart.
    public IActionResult GetViewCartModel()
        {
        // Get the current view model of the cart
        var ViewCartModel = _cartService.GetViewCartModel();

        // Return the current cart view model
        return Ok(ViewCartModel);
     }

    }


//<summary>
// Adds a product to the user's shopping cart.
// Validates the product data and checks if the product exists in the database.
// If valid, it creates a new cart item and adds it to the cart.
//</summary>

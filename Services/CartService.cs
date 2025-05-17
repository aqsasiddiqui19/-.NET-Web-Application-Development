using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Ecommerce_API_Project.Helpers;
using Newtonsoft.Json;

namespace Ecommerce_API_Project.Services
{
    public class CartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const string CartSessionKeyPrefix = "Cart_"; // Define the prefix
        private readonly ApplicationDbContext _context;
        public CartService(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context)
        {
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        // Modify to return Guid instead of string

        public Guid GetOrCreateCartId()
        {
            var cartIdString = _httpContextAccessor.HttpContext.Session.GetString("CartId");

            if (string.IsNullOrEmpty(cartIdString) || !Guid.TryParse(cartIdString, out Guid cartId))
            {
                cartId = Guid.NewGuid();
                _httpContextAccessor.HttpContext.Session.SetString("CartId", cartId.ToString());
            }

            return cartId;
        }

        // Retrieve cart from session using its CartId
        public Task<Cart?> GetCartByCartId(Guid cartId)
        {
            var session = _httpContextAccessor.HttpContext?.Session;

            if (session == null)
                return Task.FromResult<Cart?>(null);

            var cartJson = session.GetString(cartId.ToString());

            if (string.IsNullOrEmpty(cartJson))
                return Task.FromResult<Cart?>(null);

            // Deserialize using Newtonsoft.Json
            var cart = JsonConvert.DeserializeObject<Cart>(cartJson);
            return Task.FromResult(cart);
        }

        // Generate session key based on cartId
        private string GetSessionKey(Guid cartId) => $"{CartSessionKeyPrefix}{cartId}";

        // Add item to cart
        public void AddToCart(Guid cartId, CartItem cartItem)
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            var key = GetSessionKey(cartId);
            var cart = session.GetObject<List<CartItem>>(key) ?? new List<CartItem>();

            var existingItem = cart.FirstOrDefault(i => i.ProductId == cartItem.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += cartItem.Quantity;
                existingItem.TotalPrice = cartItem.Quantity * cartItem.FinalAmount;
            }
            else
            {
                cart.Add(new CartItem
                {
                    CartId = cartId,    
                    CartItemId = Guid.NewGuid(),
                    ProductId = cartItem.ProductId,
                    ProductName = cartItem.ProductName,
                    Quantity = cartItem.Quantity,
                    FinalAmount = cartItem.FinalAmount,
                    TotalPrice = cartItem.FinalAmount * cartItem.Quantity
                });
            }

            session.SetObject(key, cart); // Save updated cart in session
        }

        // Increment quantity of a specific item in the cart
        public void IncrementItem(Guid cartId, Guid productId)
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            var key = GetSessionKey(cartId);
            var cart = session.GetObject<List<CartItem>>(key);

            if (cart == null) return;

            var cartItem = cart.FirstOrDefault(x => x.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity++;
                cartItem.TotalPrice = cartItem.Quantity * cartItem.FinalAmount;

                session.SetObject(key, cart); // Save updated cart back to session
            }
        }

        // Decrement quantity of a specific item in the cart
        public void DecrementItem(Guid cartId, Guid productId)
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            var key = GetSessionKey(cartId);
            var cart = session.GetObject<List<CartItem>>(key);

            if (cart == null) return;

            var cartItem = cart.FirstOrDefault(x => x.ProductId == productId);
            if (cartItem != null)
            {
                cartItem.Quantity--;

                if (cartItem.Quantity <= 0)
                {
                    cart.Remove(cartItem); // Remove item completely
                }
                else
                {
                    cartItem.TotalPrice = cartItem.Quantity * cartItem.FinalAmount; // Update price only if not removed
                }
            }

            session.SetObject(key, cart); // Save updated cart in session
        }


        // Retrieve cart items for the given cartId
        public List<CartItem> GetCartItems(Guid cartId)
        {
            var session = _httpContextAccessor.HttpContext?.Session;
            return (session == null || cartId == Guid.Empty)
                ? new List<CartItem>()
                : session.GetObject<List<CartItem>>(GetSessionKey(cartId)) ?? new List<CartItem>();
        }
        public void RemoveItem(Guid cartId, Guid productId)
        {
            var session = _httpContextAccessor.HttpContext!.Session;
            var key = GetSessionKey(cartId);
            var cart = session.GetObject<List<CartItem>>(key);

            if (cart == null) return;

            var itemToRemove = cart.FirstOrDefault(x => x.ProductId == productId);
            if (itemToRemove != null)
            { 
                cart.Remove(itemToRemove);
                session.SetObject(key, cart);
            }
        }

        public void ClearCart(Guid cartId)
        {
            var key = GetSessionKey(cartId);
            _httpContextAccessor.HttpContext!.Session.Remove(key);
        }
        public ViewCartModel GetViewCartModel()
        {
            var cartId = GetOrCreateCartId();
            var items = GetCartItems(cartId);

            var totalItems = items.Sum(c => c.Quantity);
            var totalPrice = items.Sum(c => c.TotalPrice);

            return new ViewCartModel
            {
                Message = "Cart retrieved successfully.",
                Cart = items,
                TotalItems = totalItems,
                TotalPrice = totalPrice
            };
        }

    }

}
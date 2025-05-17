using Ecommerce_API_Project.Models;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class ViewCartModel
    {
        public string Message { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
        public List<CartItem> Cart { get; set; } = new();
    }
}

using Ecommerce_API_Project.Models;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_API_Project.DataTransferObject
{
    public class ViewCart
    {
        public Guid CartId { get; set; }
        public int TotalItems { get; set; }

        [Precision(18, 4)]
        public decimal TotalPrice { get; set; }
        public List<CartItem> Cart { get; set; } = new();
    }

}
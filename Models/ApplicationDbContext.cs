using Microsoft.EntityFrameworkCore;

namespace Ecommerce_API_Project.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Price> Prices { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Cart> Carts { get; set; }  
        public DbSet<CartItem> CartItems { get; set; }      
        public DbSet<User> Users { get; set; }
        public DbSet<Checkout> Checkouts { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Order> Orders { get; set; }    
        public DbSet<OrderItem> OrderItems { get; set; }   
    }
}

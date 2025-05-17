using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace EcommercePro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Initializes a new instance of the CartController class.
        // This constructor is used to inject dependencies such as the database context.
        //context:The ApplicationDbContext instance used to interact with the database.
        public PriceController(ApplicationDbContext context)
        {
            // Assign the injected ApplicationDbContext to the local context variable
            _context = context;
        }

        // GET: api/price/get-all-price
        [HttpGet("get-all-price")]

        // Fetches all price records from the database
        public async Task<IActionResult> GetPrice()
        {
            var prices = await _context.Prices.ToListAsync();
            return Ok(prices);
        }

        // POST: api/price/add-price
        [HttpPost("add-price")]

        // Adds a new price entry with validation
        public async Task<IActionResult> Price([FromForm] PriceViewModel price)
        {
            // Validate the incoming model
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Data Provided" });
            }

            // Check if the AdminId exists in the Admins table
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.AdminId == price.AdminId);
            if (admin == null)
            {
                return BadRequest("Invalid Admin ID.");
            }

            // Check if the price is at least 500 for admin users
            if (admin.Role?.Trim().ToLower() == "admin" && price.BasePrice < 500)
            {
                return BadRequest("Admins cannot set a price lower than 500.");
            }

            // Validate the BasePrice
            if (price.BasePrice <= 0)
            {
                return BadRequest(new { message = "Base price must be greater than 0" });
            }

            // Validate the BasePrice
            if (price.BasePrice > 100000)
            {
                return BadRequest("Base price must not exceed 100,000."); 
            }

            // Validate Discount Percentage
            if (price.DiscountPercentage < 0 || price.DiscountPercentage >= 100)
            {
                return BadRequest(new { message = "Discount percentage should be less than 100%." });
            }

            // Validate Tax Percentage
            if (price.TaxPercentage < 0 || price.TaxPercentage >= 100)
            {
                return BadRequest(new { message = "Tax percentage should be less than 100%." });
            }

            // Calculate DiscountedPrice and FinalAmount
            var basePrice = price.BasePrice;
            var discountPercentage = price.DiscountPercentage;
            var taxPercentage = price.TaxPercentage;

            var discountedPrice = basePrice - (basePrice * discountPercentage / 100);
            var finalAmount = discountedPrice + (discountedPrice * taxPercentage / 100);

            // Create a new Price record
            var newPrice = new Price
            {
                PriceId = Guid.NewGuid(),
                BasePrice = basePrice,
                DiscountPercentage = discountPercentage,
                TaxPercentage = taxPercentage,
                DiscountedPrice = discountedPrice,
                FinalAmount = finalAmount,
                AdminId=price.AdminId,
                CreatedAt = DateTime.UtcNow,
                EndDate = null,
            };

            // Add and save the new price
            _context.Prices.Add(newPrice); // <-- this line is essential
            await _context.SaveChangesAsync();

            // Return the newly created price as JSON
            string json = JsonConvert.SerializeObject(newPrice);
            return Content(json, "application/json");
        }

        // GET: api/price/get-price-by-id
        [HttpGet("get-price-by-id")]

        // Retrieves a specific price by its ID
        public async Task<IActionResult> GetPriceById(Guid id)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price == null)
            {
                return NotFound(new { message = "Price not found" });
            }

            return Ok(new { message = "PriceId fetched succesfully", GetPriceById = price });
        }

        [HttpPut("put-price-by-id")]

        // Updates an existing price record with new values
        public async Task<IActionResult> UpdatePrice(Guid id, [FromForm] PriceViewModel price)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Data Provided" });
            }

            // Find the existing price entry
            var existingPrice = await _context.Prices.FindAsync(id);
            if (existingPrice == null)
            {
                return NotFound(new { message = "Price not found" });
            }

            // Check if the AdminId exists in the Admins table
            var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == price.AdminId);
            if (!adminExists)
            {
                return BadRequest(new { message = "Invalid Admin ID. No such admin exists." });
            }

            // Validate individual fields again
            if (price.BasePrice <= 0)
            {
                return BadRequest(new { message = "Base price must be greater than 0" });
            }

            if (price.DiscountPercentage < 0 || price.DiscountPercentage >= 100)
            {
                return BadRequest(new { message = "Discount percentage must be between 0 and 100." });
            }

            if (price.TaxPercentage < 0 || price.TaxPercentage >= 100)
            {
                return BadRequest(new { message = "Tax percentage must be between 0 and 100." });
            }

            // Recalculate price fields
            var basePrice = price.BasePrice;
            var discountPercentage = price.DiscountPercentage;
            var taxPercentage = price.TaxPercentage;

            var discountedPrice = basePrice - (basePrice * discountPercentage / 100);
            var finalAmount = discountedPrice + (discountedPrice * taxPercentage / 100);

            // Update the existing price record
            existingPrice.BasePrice = basePrice;
            existingPrice.DiscountPercentage = discountPercentage;
            existingPrice.TaxPercentage = taxPercentage;
            existingPrice.DiscountedPrice = discountedPrice;
            existingPrice.FinalAmount = finalAmount;
            existingPrice.UpdatedAt = DateTime.UtcNow; // Prefer using UpdatedAt for changes
            existingPrice.AdminId=price.AdminId;

            // Save changes if needed (depending on context)
            await _context.SaveChangesAsync();

            return Ok(new { message = "Price updated successfully" });
        }

        // DELETE: /prices/{id}
        [HttpDelete("delete-price-by-id")]

        // Deletes a price record based on its ID
        public async Task<IActionResult> DeletePrice(int id)
        {
            var price = await _context.Prices.FindAsync(id);
            if (price == null)
            {
                return NotFound();
            }
            // Remove and save changes
            _context.Prices.Remove(price);
            await _context.SaveChangesAsync();

            // Return a success message
            return Ok(new { message = "Price successfully deleted." });
        }

        }

}
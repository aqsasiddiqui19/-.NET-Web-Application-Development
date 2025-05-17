using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPIProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        //Initializes a new instance of the CartController class.
        //This constructor is used to inject dependencies such as the database context.

        //context:The ApplicationDbContext instance used to interact with the database.
        //webHostEnvironment: The IWebHostEnvironment instance used to access web hosting environment properties.
        //webHostEnvironment.WebRootPath:The path to the wwwroot folder,where public files(like images, CSS, JavaScript) are stored.

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("get-all-products")]
        // Retrieves all products from the database
        public async Task<IActionResult> GetProducts()
        {
            var categories = await _context.Products.ToListAsync();
            return Ok(categories);
        }

        // POST:api/product/add-product
        [HttpPost("add-product")]

        // Creates a new category with the provided product data and image file
        public async Task<IActionResult> Product([FromForm] ProductViewModel product, IFormFile? imageFile)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }

            // Validate ProductName
            if (string.IsNullOrWhiteSpace(product.ProductName) || product.ProductName.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Product name cannot be 'string' or empty." });
            }

            // Validate ProductDescription
            if (string.IsNullOrWhiteSpace(product.ProductDescription) || product.ProductDescription.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Product description cannot be 'string' or empty." });
            }

            // Validate CategoryName
            if (string.IsNullOrWhiteSpace(product.CategoryName) || product.CategoryName.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Category name cannot be 'string' or empty." });
            }

            if (product.ProductStock < 0)
            {
                return BadRequest("Stock cannot be negative.");
            }

            // Validate that product stock is within the allowed range (1 to 10,000)

            if (product.ProductStock <= 0 || product.ProductStock > 10000)
            {
                return BadRequest(new { message = "Product stock must be between 1 and 10,000." });
            }


            // Check if the AdminId exists in the Admins table
            var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == product.AdminId);
            if (!adminExists)
            {
                return BadRequest(new { message = "Invalid Admin ID. No such admin exists." });
            }

            // Check if product name already exists
            var productExists = await _context.Products.AnyAsync(p => p.ProductName == product.ProductName);
            if (productExists)
            {
                return Conflict(new { message = "A product with this name already exists." });
            }

            // Check if product description already exists
            var productDescriptionExists = await _context.Products.AnyAsync(p => p.ProductDescription == product.ProductDescription);
            if (productDescriptionExists)
            {
                return Conflict(new { message = "A product with this description already exists." });
            }

            // Validate CategoryName exists in the database
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == product.CategoryName);
            if (category == null)
            {
                return BadRequest(new { message = "The selected category does not exist. Please choose an existing category." });
            }

            // Validate PriceId exists in the database
            var price = await _context.Prices.FirstOrDefaultAsync(p => p.PriceId == product.PriceId);
            if (price == null)
            {
                return BadRequest(new { message = "Invalid price selection." });
            }

            // Ensure that an image file is provided
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Image file is required.");


            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
            string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

            // Validate file extension
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest(new { message = "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed." });
            }

            // Define the folder to save images and create it if it doesn't exist
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string originalFileName = Path.GetFileNameWithoutExtension(imageFile.FileName);

            // Generate a unique filename for the image using GUID
            string newFileName = $"{originalFileName}{fileExtension}";
            string filePath = Path.Combine(uploadsFolder, newFileName);
            var newImageUrl = "/uploads/" + newFileName;

            // Ensure the image name is unique
            var imageExists = await _context.Categories.AnyAsync(c => c.ImageUrl == newImageUrl);
            if (imageExists)
            {
                return Conflict(new { message = "An image with the same name already exists. Please upload a new image file." });
            }

            // Save the uploaded image file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            // Store relative path
            string imageUrl = "/uploads/" + newFileName;

            // Create a new Product entity
            var productEntity = new Product
            {
                ProductId = Guid.NewGuid(),
                ProductName = product.ProductName,
                ProductDescription = product.ProductDescription,
                ProductStock = product.ProductStock,
                CategoryName = product.CategoryName,
                AdminId = product.AdminId,
                CategoryId = product.CategoryId,
                PriceId = product.PriceId,
                ImageUrl = imageUrl,
                TaxPercentage = price.TaxPercentage,
                DiscountPercentage = price.DiscountPercentage,
                DiscountedPrice = price.DiscountedPrice,
                FinalAmount = price.FinalAmount,
            };

            // Save product to database
            _context.Products.Add(productEntity);
            await _context.SaveChangesAsync();

            // Return the created product (you can adjust this response based on your need)
            return Ok(new { message = "Product created successfully", product = productEntity });
        }

        //PUT:api/product/put-product-by-id
        [HttpPut("put-product-by-id")]
        public async Task<IActionResult> UpdateProduct([FromForm] ProductViewModel product, Guid id, IFormFile? imageFile)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Data Provided" });
            }

            // Find existing product by ID
            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            // Check if the AdminId exists in the Admins table
            var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == product.AdminId);
            if (!adminExists)
            {
                return BadRequest(new { message = "Invalid Admin ID. No such admin exists." });
            }

            // Check if product name already exists
            var existingProductName = await _context.Products.AnyAsync(p => p.ProductName == product.ProductName);
            if (existingProductName)
            {
                return Conflict(new { message = "A product with this name already exists." });
            }

            // Check if product description already exists
            var existingproductdesscription = await _context.Products.AnyAsync(p => p.ProductDescription == product.ProductDescription);
            if (existingproductdesscription)
            {
                return Conflict(new { message = "A product with this description already exists." });
            }

            // Check if the given category exists
            var existingcategory = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryName == product.CategoryName);

            // Validate category
            if (existingcategory == null)
            {
                return BadRequest(new { message = "The selected category does not exist. Please choose an existing category." });
            }

            // Validate price
            var price = await _context.Prices.FirstOrDefaultAsync(p => p.PriceId == product.PriceId);
            if (price == null)
            {
                return BadRequest(new { message = "Invalid price selection." });
            }

            // Handle image update
            if (imageFile != null)
            {
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                // Validate file extension
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed." });
                }


                var uploadsFolder = Path.Combine("wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                var FileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string newFileName = $"{FileName}{fileExtension}";
                var newFilePath = Path.Combine(uploadsFolder, newFileName);
                var newImageUrl = "/uploads/" + newFileName;


                // Check if a similar image exists
                var imageExists = await _context.Categories.AnyAsync(c => c.ImageUrl == newImageUrl);
                if (imageExists)
                {
                    return Conflict(new { message = "An image with the same name already exists. Please upload a new image file." });
                }

                //Replace old image with new image if exits
                if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                {
                    string oldFilePath = Path.Combine("wwwroot", existingProduct.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        Console.WriteLine($"Deleting old file: {oldFilePath}");
                        System.IO.File.Delete(oldFilePath);
                    }
                    else
                    {
                        Console.WriteLine($"Old file not found: {oldFilePath}");
                    }
                }

                // Save new image
                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                existingProduct.ImageUrl = newImageUrl; // Store relative path in database
            }

            // Update the product details
            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductDescription = product.ProductDescription;
            existingProduct.ProductStock = product.ProductStock;
            existingProduct.FinalAmount = price.FinalAmount;
            existingProduct.DiscountPercentage = price.DiscountPercentage;
            existingProduct.DiscountedPrice = price.DiscountedPrice;
            existingProduct.TaxPercentage = price.TaxPercentage;
            existingProduct.AdminId = product.AdminId;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.PriceId = price.PriceId;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Product created successfully", product = existingProduct });

        }

        //Get:api/product/get-product-by-id
        [HttpGet("get-product-by-id")]

        // Get product by ID
        public async Task<IActionResult> GetProductById(Guid id)
        {
            // Find product by ID
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            return Ok(new { message = "Category fetched successfully.", product });
        }

        //Delete:api/product/delete-product-by-id
        [HttpDelete("delete-product-by-id")]

        //Delete product by ID
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            // Find product by ID
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Product not found." });
            }

            // Delete the product
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product successfully deleted." });

        }
    }
}

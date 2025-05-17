using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_API_Project.Controllers
{
    [ApiController]
    [Route("api/categories")]

    // Constructor to inject dependencies: ApplicationDbContext and IWebHostEnvironment
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // Initializes a new instance of the CartController class.
        // This constructor is used to inject dependencies such as the database context.
        //context:The ApplicationDbContext instance used to interact with the database.
        //webHostEnvironment: The IWebHostEnvironment instance used to access web hosting environment properties.
        //webHostEnvironment.WebRootPath:The path to the wwwroot folder,
        //where public files(like images, CSS, JavaScript) are stored.

        public CategoryController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            // Assign the injected ApplicationDbContext to the local context variable
            _context = context;

            // Assign the injected IWebHostEnvironment to the local _webHostEnvironment variable
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/category/get-all-category
        [HttpGet("get-all-category")]

        // Retrieves all categories from the database
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _context.Categories.ToListAsync();
            return Ok(categories); // Return categories as a response
        }

        // POST:api/category/add-category
        [HttpPost("add-category")]

        // Creates a new category with the provided category data and o image file
        public async Task<IActionResult> CreateCategory([FromForm] CategoryViewModel category, IFormFile? imageFile)
        {
            // Check if the model state is valid
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided" });
            }

            // Validate CategoryName: not null, not empty, not whitespace and prevent "string" or dummy values
            if (string.IsNullOrWhiteSpace(category.CategoryName) || category.CategoryName.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Category Name cannot be 'string' or empty." });
            }

            // Validate BrandDescription: not null, not empty, not whitespace and prevent "string" or dummy values
            if (string.IsNullOrWhiteSpace(category.CategoryDescription) || category.CategoryDescription.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Category Description cannot be 'string' or empty." });
            }
            
            // Check if the AdminId exists in the Admins table
            var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == category.AdminId);
            if (!adminExists)
            {
                return BadRequest(new { message = "Invalid Admin ID. No such admin exists." });
            }

            // Check if a category with the same name already exists
            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryName == category.CategoryName);
            if (categoryExists)
            {
                return Conflict(new { message = "Category already exists." });
            }

            // Check if a category with the same description already exists
            var categoryDescriptionExist = await _context.Categories .AnyAsync(c => c.CategoryDescription == category.CategoryDescription);
            if (categoryDescriptionExist)
            {
                return Conflict(new { message = "Category description already exists." });
            }

            // Ensure that an image file is provided
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Image file is required.");


            // Validate the file type for the image
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

            // Create and store the new category with the uploaded image URL
            var newCategory = new Category
            {
                CategoryId = Guid.NewGuid(),
                AdminId = category.AdminId,
                CategoryName = category.CategoryName,
                CategoryDescription = category.CategoryDescription,
                ImageUrl = imageUrl
            };
            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();

            // Return the newly created category and a success message
            return Ok(new { message = "Category added successfully.", category = newCategory });
        }

        // GET: api/Category/get-category-by-id
        [HttpGet("get-category-by-id")]

        // Retrieves a category by its ID
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            // Find the category by its ID
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }
            // Return the category and a success message
            return Ok(new { message = "Category fetched successfully.", category });
        }

        // Update category by ID
        [HttpPut("update-category-by-id")]

        // Updates a category by its ID with new data and optionally a new image
        public async Task<IActionResult> UpdateCategory(Guid id, [FromForm] CategoryViewModel category, IFormFile? imageFile)
        {
            // Find the existing category by its ID
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
            {
                return NotFound(new { message = "Category not found." });
            }

            // Check if another category has the same name (excluding the current category)
            var categoryNameExist = await _context.Categories
                .AnyAsync(c => c.CategoryName == category.CategoryName);
            if (categoryNameExist)
            {
                return Conflict(new { message = "Category name already exists." });
            }

            // Check if another category has the same description (excluding the current category)
            var categoryDescriptionExist = await _context.Categories
                .AnyAsync(c => c.CategoryDescription == category.CategoryDescription);
            if (categoryDescriptionExist)
            {
                return Conflict(new { message = "Category description already exists." });
            }

            // Update category details
            existingCategory.CategoryName = category.CategoryName;
            existingCategory.CategoryDescription = category.CategoryDescription;


            // Handle image update
            if (imageFile != null)
            {
                // Validate the new image file
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                // Validate file extension
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { message = "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed." });
                }

                // Define the folder for image uploads
                var uploadsFolder = Path.Combine("wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

                // Generate a unique filename for the new image
                var FileName = Path.GetFileNameWithoutExtension(imageFile.FileName);
                string newFileName = $"{FileName}{fileExtension}";
                var newFilePath = Path.Combine(uploadsFolder, newFileName);
                var newImageUrl = "/uploads/" + newFileName;

                // Check if the image name is unique
                var imageExists = await _context.Categories.AnyAsync(c => c.ImageUrl == newImageUrl);
                if (imageExists)
                {
                    return Conflict(new { message = "An image with the same name already exists. Please upload a new image file." });
                }

                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(existingCategory.ImageUrl))
                {
                    string oldFilePath = Path.Combine("wwwroot", existingCategory.ImageUrl.TrimStart('/'));
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

                // Save the new image file
                using (var stream = new FileStream(newFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }
                // Update the category with the new image URL
                existingCategory.ImageUrl = newImageUrl; // Store relative path in database
            }

            // Save changes to the database
            await _context.SaveChangesAsync();
            return Ok(new { message = "Category updated successfully." });
        }

        // Delete category by ID
        [HttpDelete("delete-category-by-id")]

        // Deletes a category by its ID
        public async Task<IActionResult> DeleteCategory(Guid id)
            {
            // Find the category by its ID
            var category = await _context.Categories.FindAsync(id);
                if (category == null)
                {
                    return NotFound(new { message = "Category not found." });
                }

            // Remove the category from the database
            _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

            // Return a success message
            return Ok(new { message = "Category successfully deleted." });
            }
        }
    }

using System;
using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_API_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Initializes a new instance of the CartController class.
        // This constructor is used to inject dependencies such as the database context
        //context:The ApplicationDbContext instance used to interact with the database.
        public BrandController(ApplicationDbContext context)
        {
            // Assign the injected ApplicationDbContext to the local context variable
            _context = context;
        }

        // GET: api/get-all-brand
        [HttpGet("get-all-brands")]

        //Retrieves all brand records from the database.
        public async Task<IActionResult> GetBrand()
        {
            // Fetch all brands from the database asynchronously.
            var brand = await _context.Brands.ToListAsync();

            // Return the list of brands with a status 200 OK
            return Ok(brand);
        }

        // POST: api/Brand/add-brand
        [HttpPost("add-brand")]

        //Adds a new brand to the database after validating the data.
        public async Task<IActionResult> CreateBrand([FromForm] BrandViewModel brand)
        {
            // If the provided data is invalid
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid Data Provided" });
            }

            // Validate BrandName: not null, not empty, not whitespace and prevent "string" or dummy values
            if (string.IsNullOrWhiteSpace(brand.BrandName) || brand.BrandName.ToLower() == "string")
            {
                return BadRequest(new { message = "Brand Name cannot be 'string' or empty." });
            }

            // Validate BrandDescription: not null, not empty, not whitespace and prevent "string" or dummy values
            if (string.IsNullOrWhiteSpace(brand.BrandDescription) || brand.BrandDescription.ToLower() == "string")
            {
                return BadRequest(new { message = "Brand Description cannot be 'string' or empty." });
            }

            // Check if the brand name already exists in the database.
            var brandName = await _context.Brands.AnyAsync(b => b.BrandName == brand.BrandName);

            // If the brand name already exists
            if (brandName)
            {
                // Return conflict message.
                return Conflict(new { message = "Brand name already exists." });
            }

            // Check if the brand description already exists in the database.
            var brandDescriptionExist = await _context.Brands.AnyAsync(b => b.BrandDescription == brand.BrandDescription);

            // If the brand desscription already exists
            if (brandDescriptionExist)
            {
                // Return conflict message.
                return Conflict(new { message = "Brand description already exists." });
            }

            // Check if the AdminId exists in the Admins table
            var adminExists = await _context.Admins.AnyAsync(a => a.AdminId == brand.AdminId);
            if (!adminExists)
            {
                return BadRequest(new { message = "Invalid Admin ID. No such admin exists." });
            }

            // Create a new Brand object with the provided data.
            var newBrand = new Brand
            {
                BrandId = Guid.NewGuid(),
                BrandName = brand.BrandName,
                BrandDescription = brand.BrandDescription,
                AdminId=brand.AdminId
            };

            // Add the new brand to the Brands table in the database.
            await _context.Brands.AddAsync(newBrand);
            
            // Save changes to the database.
            await _context.SaveChangesAsync();

            // Return success message.
            return Ok(new { message = "Brand Successfully Added", brand = newBrand });
        }

        // GET: api/Brand/get-brand-by-id
        [HttpGet("get-brand-by-id")]

        // Retrieves a brand by its unique ID from the database.
        public async Task<IActionResult> GetBrandById(Guid id)
        {
            // Find the brand by ID in the database.
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                // Return error if brand is not found.
                return NotFound(new { message = "Brand not found" });
            }

            // Return the found brand details.
            return Ok(new { message = "BrandId fetched succesfully", GetBrandById = brand });
        }

        // PUT: api/Brand/update-brand-by-Id
        [HttpPut("update-brand-by-Id")]

        // Updates an existing brand's details by its unique ID.
        public async Task<IActionResult> UpdateBrandById(Guid id, [FromForm] BrandViewModel brand)
        {
            // Find the brand to update.
            var existingBrand = await _context.Brands.FindAsync(id);
            if (existingBrand == null)
            {
                // Return error if brand does not exist.
                return NotFound("Brand not found.");
            }

            // If the provided data is invalid
            if (!ModelState.IsValid)
            {
                // Return error message for invalid data.
                return BadRequest(new { message = "Invalid data provided" });
            }

            // Validate BrandName: not null, not empty, not whitespace and prevent "string" or dummy values
            if (string.IsNullOrWhiteSpace(brand.BrandName) || brand.BrandName.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Brand Name cannot be 'string' or empty." });
            }

            // Validate BrandDescription: not null, not empty, not whitespace and prevent "string" or dummy values
            if (string.IsNullOrWhiteSpace(brand.BrandDescription) || brand.BrandDescription.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Brand Description cannot be 'string' or empty." });
            }

            // Check if the brand name already exists in the database.
            var brandNameExist = await _context.Brands.AnyAsync(b => b.BrandName == brand.BrandName);

            // If the brand name already exists
            if (brandNameExist)
            {
                // Return conflict message.
                return Conflict(new { message = "Brand name already exists." });
            }

            // Check if the brand description already exists in the database.
            var brandDescriptionExist = await _context.Brands.AnyAsync(b => b.BrandDescription == brand.BrandDescription);

            // If the brand desscription already exists
            if (brandDescriptionExist)
            {
                // Return conflict message.
                return Conflict(new { message = "Brand description already exists." });
            }

            // Check if the AdminId exists in the Admins table
            var adminExists = await _context.Brands.AnyAsync(a => a.AdminId == brand.AdminId);
            if (!adminExists)
            {
                return BadRequest(new { message = "Invalid Admin ID. No such admin exists." });
            }

            // Update the BrandName
            existingBrand.BrandName = brand.BrandName;
            existingBrand.BrandDescription = brand.BrandDescription;

            // Update the brand in the database.
            _context.Brands.Update(existingBrand);

            // Save changes to the database.
            await _context.SaveChangesAsync();

            // Return success message with updated brand.
            return Ok(new { message = "Brand updated successfully ", UpdatedCategory = existingBrand });
        }

        // Delete: api/Brand/delete-brand-by-id
        [HttpDelete("delete-brand-by-id")]

        // Deletes a brand by its unique ID from the database.
        public async Task<IActionResult> DeleteBrandById(Guid id)
        {
            // Find the brand to delete.
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                // Return error if brand is not found.
                return NotFound(new { message = "Brand not found." });
            }

            // Remove the brand from the database.
            _context.Brands.Remove(brand);

            // Save changes to delete the brand.
            await _context.SaveChangesAsync();

            // Return success message.
            return Ok(new { message = "Brand successfully deleted." });
        }

    }
}

//Good API design rule:
//API routes should always be lowercase and hyphen-separated not PascalCase or camelCase.
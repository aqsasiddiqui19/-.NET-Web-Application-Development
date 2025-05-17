using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_API_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Initializes a new instance of the CartController class.
        // This constructor is used to inject dependencies such as the database context
        //context:The ApplicationDbContext instance used to interact with the database.
        public AdminController(ApplicationDbContext context)
        {
            // Assign the injected ApplicationDbContext to the local context variable
            _context = context;
        }

        // GET: api/get-all-admin
        [HttpGet("get-all-admin")]
        public async Task<ActionResult> GetAllAdmins()
        {
            var admins = await _context.Admins.ToListAsync();
            return Ok(admins);
        }

        // POST: api/Admin/register
        [HttpPost("register")]

        // Registers a new admin after validating the data (admin name, email).
        public async Task<IActionResult> Register([FromForm] AdminViewModel model)
        {
            // If the provided data is invalid
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Invalid data provided." });
            }

            // Validate FullName: not null, not empty, not whitespace, not a placeholder
            if (string.IsNullOrWhiteSpace(model.FullName) || model.FullName.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Full name cannot be 'string' or empty." });
            }

            // Check if the admin name already exists in the database.
            if (await AdminNameExist(model.FullName))
            {
                return BadRequest(new { message = "Admin name already exists" });
            }

            // Check if the email already exists in the database.
            if (await EmailExist(model.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            // Create a new Admin object with the provided data.
            var admin = new Admin
            {
                AdminId = Guid.NewGuid(), 
                FullName = model.FullName,
                Email = model.Email,
                Password = HashPassword(model.Password),
                Role = "Admin" // Set default role to "Admin".
            };

            // Add the new admin to the Admins table in the database.
            _context.Admins.Add(admin);

            // Save changes to the database.
            await _context.SaveChangesAsync();

            // Return success message.
            return Ok(new { message = "Admin registered successfully" });
        }

        //Checks if an name already exists in the database.
        private async Task<bool> AdminNameExist(string fullName)
        {
            return await _context.Admins.AnyAsync(a => a.FullName == fullName);
        }

        //Checks if an email already exists in the database.
        private async Task<bool> EmailExist(string email)
        {
            return await _context.Admins.AnyAsync(a => a.Email == email);
        }

        // Hashes the admin's password using BCrypt.
        private string HashPassword(string? password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // GET: api/Admin/get-admin-by-id
        // Retrieves an admin by their unique ID from the database.
        [HttpGet("get-admin-by-id")]
        public async Task<IActionResult> GetAdminById(Guid id)
        {
            // Find the admin to delete.
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                // Return error if admin does not exist.
                return NotFound(new { message = "Admin not found." });
            }
            return Ok(admin);
        }

        // PUT: api/Admin/update-admin-by-id
        //Updates an existing admin's details by their unique ID.
        [HttpPut("update-admin-by-id")]
        public async Task<IActionResult> UpdateAdminById(Guid id, [FromForm] AdminViewModel updatedAdmin)
        {
            // Find the admin to delete.
            var existingAdmin = await _context.Admins.FindAsync(id);
            if (existingAdmin == null)
            {

            // Return error if admin is not found.
                return NotFound(new { message = "Admin not found." });
            }

            // Update the admin details with the provided data.
            existingAdmin.FullName = updatedAdmin.FullName;
            existingAdmin.Email = updatedAdmin.Email;

            // Update the admin in the database.
            _context.Admins.Update(existingAdmin);

            // Save changes.
            await _context.SaveChangesAsync();

            return Ok(new { message = "Admin updated successfully.", admin = existingAdmin });
        }

        // DELETE: api/Admin/delete-admin-by-Id
        [HttpDelete("delete-admin-by-Id")]
        public async Task<IActionResult> DeleteAdminById(Guid id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
            // Return error if admin is not found.
                return NotFound(new { message = "Admin not found." });
            }
            // Remove the admin from the database.
            _context.Admins.Remove(admin);

            // Save changes to delete the admin.
            await _context.SaveChangesAsync();

            // Return success message.
            return Ok(new { message = "Admin deleted successfully." });
        }


        // POST: api/Admin/Login
        [HttpPost("AdminLogin")]
        public async Task<IActionResult> AdminLogin([FromForm] LoginVeiwModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Username and Password are not valid" });
            }

            // Check if an admin with the provided username exists in the database
            var admin = await _context.Admins.SingleOrDefaultAsync(a => a.FullName == model.UserName);

            // If the admin is not found, return an error
            if (admin == null)
            {
                return BadRequest(new { message = "Invalid Admin Username" });
            }

            // Verify the provided password against the stored hashed password
            if (!VerifyPassword(model.Password, admin.Password))
            {
                return BadRequest(new { message = "Invalid Admin Password" });
            }

            // Return a success message
            return Ok(new { message = "Admin login successfully...", username = admin.FullName, role = "Admin" });
        }

        // Helper method to verify a hashed password
        private bool VerifyPassword(string? userFrontPassword, string? userDatabasePassword)
        {
            // Use BCrypt to verify the password
            return BCrypt.Net.BCrypt.Verify(userFrontPassword, userDatabasePassword);
        }
    }
}
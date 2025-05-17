using Ecommerce_API_Project.DataTransferObject;
using Ecommerce_API_Project.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Ecommerce_API_Project.Controllers
{
    [Route("api/[controller]")] // URl: api/controller/register
    [ApiController]

    // CREATE REGISTERATION API
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Constructor to initialize ApplicationDbContext through dependency injection
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: api/User/get-all-user
        [HttpGet("get-all-user")]
       
        // Retrieve all users from the database
        public async Task<ActionResult> GetAllUsers()
        {
            var users = await _context.Users.ToListAsync();

            // Return the list of users with a 200 OK response
            return Ok(users);
        }

        //POST: api/User/register
        [HttpPost("register")]

        // (*FromBody* use for fetch data from JSON or XML)
        // Reference variable *model*
        // UserViewModel use for validate data comes from user 
        public async Task<IActionResult> Register([FromForm] UserVeiwModel model)
        {
            // Validate the incoming user data using model validation rules
            if (!ModelState.IsValid) 
            {
                return BadRequest(new { message = "Invalid data provided" });
            }

            // Validate FullName: not null, not empty, not whitespace, not a placeholder
            if (string.IsNullOrWhiteSpace(model.UserName) || model.UserName.Trim().ToLower() == "string")
            {
                return BadRequest(new { message = "Full name cannot be 'string' or empty." });
            }

            // Check if the username already exists in the database
            if (await UserNamelExist(model.UserName))
            {
                return BadRequest(new { message = "UserName already Exist" });
            }

            // Check if the email already exists in the database
            if (await EmailExist(model.Email))
            {
                return BadRequest(new { message = "Email already Exist" });
            }

            // Create a new User object
            var user = new User
            {
                UserId = Guid.NewGuid(),
                UserName = model.UserName,
                Email = model.Email,
                Password = HashPassword(model.Password),
            };

            // Add the newly created user to the database context
            _context.Users.Add(user);

            // Save the changes asynchronously to the database
            await _context.SaveChangesAsync();

            // Return a success response
            return Ok(new { message = "Registration Sucessfully" });
        }
        
        // Methods:

        // Method to check if a username already exists in the database
        private async Task<bool> UserNamelExist(string username)
        {
            return await _context.Users.AnyAsync(backendUserName => backendUserName.UserName == username);
        }

        // Method to check if an email already exists in the database
        private async Task<bool> EmailExist(string email)
        {
            return await _context.Users.AnyAsync(backendemail => backendemail.Email == email);
        }

        // Method to hash a plain text password using BCrypt
        private string HashPassword(string? password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // GET: api/User/{id}
        [HttpGet("get-user-by-id")]

        // Retrieve a user from the database using their ID
        public async Task<IActionResult> GetUserById(Guid id)
        {
            //find the user by ID in the database
            var user = await _context.Users.FindAsync(id);

            // If the user is not found, return a 404 Not Found response
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // If user is found, return the user data with 200 OK status
            return Ok(user);
        }

        // PUT: api/User/put-user-by-id
        [HttpPut("put-user-by-id")]

        // Updates an existing user's information based on their unique ID
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] User updatedUser)
        {
            //find the existing user by ID
            var existingUser = await _context.Users.FindAsync(id);

            // If the user is not found, return a 404 Not Found response
            if (existingUser == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Update the user's fields with the new values
            existingUser.UserName = updatedUser.UserName;
            existingUser.Email = updatedUser.Email;

            //user entity as updated in the database context
            _context.Users.Update(existingUser);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            // Return a success response
            return Ok(new { message = "User updated successfully.", user = existingUser });
        }

        // DELETE: api/User/delete-user-by-id
        [HttpDelete("delete-user-by-id")]

        // Deletes a user from the database by ID
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            //find the user by ID in the database
            var user = await _context.Users.FindAsync(id);

            // If the user is not found, return a 404 Not Found response
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // If user is found, remove the user from the database
            _context.Users.Remove(user);

            // Save changes to the database
            await _context.SaveChangesAsync();

            // Return a success respons
            return Ok(new { message = "User deleted successfully." });
        }


        //CREATE USER LOGIN API//
        
        // POST: api/User/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] LoginVeiwModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "UserName and Password are not Valid" });
            }

            // Check if a user with the provided username exists in the database
            var user = await _context.Users.SingleOrDefaultAsync(udata => udata.UserName == model.UserName);

            // If the user is not found, return an error
            if (user == null)
            {
                return BadRequest(new { message = "InValid Username" });
            }

            // Verify the provided password against the stored hashed password
            if (!VerifyPassword(model.Password, user.Password))
            {
                return BadRequest(new { message = "Invalid Password" });
            }
            // Return a success message
            return Ok(new { message = "Login sucessfully...", username = user.UserName, role = user.Role });
        }

        // Helper method to verify a hashed password
        private bool VerifyPassword(string? userfrontpassword, string? userdatabasepassword)
        {
            // Use BCrypt to verify the password
            return BCrypt.Net.BCrypt.Verify(userfrontpassword, userdatabasepassword);
        }
    }
}








//Note:
//class mein value ko intialization
//CREATE A OBJECT USE NEW AND DATA SAVE INTO THE (USER).
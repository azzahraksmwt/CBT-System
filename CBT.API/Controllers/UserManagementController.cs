using CBT.API.Auth;
using CBT.API.DbContext;
using CBT.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CBT.API.Controllers
{
    [Route("api/[controller]")]
    //[Authorized]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly CBTContext _context;

        public UserManagementController(CBTContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateUserManagement([FromBody] UserManagement userManagement)
        {
            try
            {
                if (userManagement == null)
                {
                    return BadRequest("UserManagement cannot be null");
                }

                userManagement.created_at = DateTime.Now;
                userManagement.updated_at = DateTime.Now;

                _context.UserManagement.Add(userManagement);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUserManagementById), new { id = userManagement.id }, userManagement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating UserManagement: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the UserManagement. Please try again later.");
            }
        }

        // Get All
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserManagement>>> GetAllUserManagements()
        {
            try
            {
                var schedules = await _context.UserManagement
                    .Where(qt => qt.deleted_at == null)
                    .ToListAsync();
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving UserManagement: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the UserManagement. Please try again later.");
            }
        }

        // Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<UserManagement>> GetUserManagementById(int id)
        {
            try
            {
                //var user = (UserManagement)HttpContext.Items["User"];

                //// Ambil userId dari klaim (contoh klaim dengan key "userId")
                //var userIdClaim = HttpContext.User?.Claims.FirstOrDefault(c => c.Type == "userId")?.Value;
                //if (userIdClaim == null || int.TryParse(userIdClaim, out int userId) == false)
                //{
                //    return Unauthorized("User is not authenticated or userId is missing.");
                //}

                var userManagement = await _context.UserManagement.FindAsync(id);
                if (userManagement == null)
                {
                    return NotFound();
                }

                return Ok(userManagement);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving UserManagement by ID: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the UserManagement. Please try again later.");
            }
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserManagement(int id, [FromBody] UserManagement userManagement)
        {
            try
            {
                //var user = (UserManagement)HttpContext.Items["User"];

                //if (user == null)
                //{
                //    return Unauthorized("User is not authenticated.");
                //}

                //if (userManagement == null || id != userManagement.id)
                //{
                //    return BadRequest("Invalid UserManagement or ID mismatch");
                //}

                var existingUserManagement = await _context.UserManagement.FindAsync(id);
                if (existingUserManagement == null)
                {
                    return NotFound();
                }

                existingUserManagement.name = userManagement.name;
                existingUserManagement.email = userManagement.email;
                existingUserManagement.password = userManagement.password;
                existingUserManagement.role = userManagement.role;
                existingUserManagement.updated_at = DateTime.Now;
                existingUserManagement.updated_by = userManagement.updated_by;

                _context.UserManagement.Update(existingUserManagement);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating UserManagement: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the UserManagement. Please try again later.");
            }
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserManagement(int id)
        {
            try
            {
                var userManagement = await _context.UserManagement.FindAsync(id);
                if (userManagement == null)
                {
                    return NotFound();
                }

                userManagement.deleted_at = DateTime.Now;

                _context.UserManagement.Update(userManagement);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting UserManagement: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the UserManagement. Please try again later.");
            }
        }
    }
}

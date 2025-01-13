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
    [ApiController]
    public class TestResultHistoryController : ControllerBase
    {
        private readonly CBTContext _context;

        public TestResultHistoryController(CBTContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateTestResultHistory([FromBody] TestResultHistory testResultHistory)
        {
            try
            {
                if (testResultHistory == null)
                {
                    return BadRequest("TestResultHistory cannot be null");
                }

                testResultHistory.created_at = DateTime.Now;
                testResultHistory.updated_at = DateTime.Now;

                _context.TestResultHistory.Add(testResultHistory);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTestResultHistoryById), new { id = testResultHistory.id }, testResultHistory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating TestResultHistory: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the TestResultHistory. Please try again later.");
            }
        }

        // Get All
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestResultHistory>>> GetAllTestResultHistorys()
        {
            try
            {
                var schedules = await _context.TestResultHistory
                    .Where(qt => qt.deleted_at == null)
                    .ToListAsync();
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving TestResultHistory: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the TestResultHistory. Please try again later.");
            }
        }

        // Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<TestResultHistory>> GetTestResultHistoryById(int id)
        {
            try
            {
                var testResultHistory = await _context.TestResultHistory.FindAsync(id);
                if (testResultHistory == null)
                {
                    return NotFound();
                }

                return Ok(testResultHistory);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving TestResultHistory by ID: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the TestResultHistory. Please try again later.");
            }
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTestResultHistory(int id, [FromBody] TestResultHistory testResultHistory)
        {
            try
            {
                if (testResultHistory == null || id != testResultHistory.id)
                {
                    return BadRequest("Invalid TestResultHistory or ID mismatch");
                }

                var existingTestResultHistory = await _context.TestResultHistory.FindAsync(id);
                if (existingTestResultHistory == null)
                {
                    return NotFound();
                }

                existingTestResultHistory.test_result_id = testResultHistory.test_result_id;
                existingTestResultHistory.question_id = testResultHistory.question_id;
                existingTestResultHistory.question_option_id = testResultHistory.question_option_id;
                existingTestResultHistory.IsCorrect = testResultHistory.IsCorrect;
                existingTestResultHistory.updated_at = DateTime.Now;
                existingTestResultHistory.updated_by = testResultHistory.updated_by;

                _context.TestResultHistory.Update(existingTestResultHistory);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating TestResultHistory: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the TestResultHistory. Please try again later.");
            }
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestResultHistory(int id)
        {
            try
            {
                var testResultHistory = await _context.TestResultHistory.FindAsync(id);
                if (testResultHistory == null)
                {
                    return NotFound();
                }

                testResultHistory.deleted_at = DateTime.Now;

                _context.TestResultHistory.Update(testResultHistory);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting TestResultHistory: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the TestResultHistory. Please try again later.");
            }
        }
    }
}

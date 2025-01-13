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
    public class TestResultController : ControllerBase
    {
        private readonly CBTContext _context;

        public TestResultController(CBTContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateTestResult([FromBody] TestResult testResult)
        {
            try
            {
                if (testResult == null)
                {
                    return BadRequest("TestResult cannot be null");
                }

                testResult.created_at = DateTime.Now;
                testResult.updated_at = DateTime.Now;

                _context.TestResult.Add(testResult);
                await _context.SaveChangesAsync();

                foreach (var history in testResult.test_result_history)
                {
                    history.test_result_id = testResult.id;
                    history.created_at = DateTime.Now;
                    history.updated_at = DateTime.Now;

                    if (history.id == 0)
                    {
                        _context.TestResultHistory.Add(history);
                    }
                    else
                    {
                        _context.TestResultHistory.Update(history);
                    }
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetTestResultById), new { id = testResult.id }, testResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating TestResult: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the TestResult. Please try again later.");
            }
        }

        // Get All
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TestResult>>> GetAllTestResults()
        {
            try
            {
                var schedules = await _context.TestResult
                    .Where(qt => qt.deleted_at == null)
                    .ToListAsync();
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving TestResult: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the TestResult. Please try again later.");
            }
        }

        // Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<TestResult>> GetTestResultById(int id)
        {
            try
            {
                var testResult = await _context.TestResult.FindAsync(id);
                if (testResult == null)
                {
                    return NotFound();
                }

                return Ok(testResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving TestResult by ID: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the TestResult. Please try again later.");
            }
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTestResult(int id, [FromBody] TestResult testResult)
        {
            try
            {
                if (testResult == null || id != testResult.id)
                {
                    return BadRequest("Invalid TestResult or ID mismatch");
                }

                var existingTestResult = await _context.TestResult
                   .Include(q => q.test_result_history)
                   .FirstOrDefaultAsync(q => q.id == id);

                if (existingTestResult == null)
                {
                    return NotFound();
                }

                existingTestResult.user_management_id = testResult.user_management_id;
                existingTestResult.question_topic_id = testResult.question_topic_id;
                existingTestResult.correct_score = testResult.correct_score;
                existingTestResult.incorrect_score = testResult.incorrect_score;
                existingTestResult.total_score = testResult.total_score;
                existingTestResult.updated_at = DateTime.Now;
                existingTestResult.updated_by = testResult.updated_by;

                _context.TestResult.Update(existingTestResult);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating TestResult: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the TestResult. Please try again later.");
            }
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTestResult(int id)
        {
            try
            {
                var testResult = await _context.TestResult.FindAsync(id);
                if (testResult == null)
                {
                    return NotFound();
                }

                testResult.deleted_at = DateTime.Now;

                _context.TestResult.Update(testResult);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting TestResult: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the TestResult. Please try again later.");
            }
        }
    }
}

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
    public class QuestionOptionController : ControllerBase
    {
        private readonly CBTContext _context;

        public QuestionOptionController(CBTContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateQuestionOption([FromBody] QuestionOption questionOption)
        {
            try
            {
                if (questionOption == null)
                {
                    return BadRequest("QuestionOption cannot be null");
                }

                questionOption.created_at = DateTime.Now;
                questionOption.updated_at = DateTime.Now;

                _context.QuestionOption.Add(questionOption);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQuestionOptionById), new { id = questionOption.id }, questionOption);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating QuestionOption: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the QuestionOption. Please try again later.");
            }
        }

        // Get All
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionOption>>> GetAllQuestionOptions()
        {
            try
            {
                var options = await _context.QuestionOption
                    .Where(qt => qt.deleted_at == null)
                    .ToListAsync();
                return Ok(options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving QuestionOptions: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the QuestionOptions. Please try again later.");
            }
        }

        // Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionOption>> GetQuestionOptionById(int id)
        {
            try
            {
                var option = await _context.QuestionOption.FindAsync(id);
                if (option == null)
                {
                    return NotFound();
                }

                return Ok(option);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving QuestionOption by ID: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the QuestionOption. Please try again later.");
            }
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestionOption(int id, [FromBody] QuestionOption questionOption)
        {
            try
            {
                if (questionOption == null || id != questionOption.id)
                {
                    return BadRequest("Invalid QuestionOption or ID mismatch");
                }

                var existingOption = await _context.QuestionOption.FindAsync(id);
                if (existingOption == null)
                {
                    return NotFound();
                }

                // Update properties
                existingOption.question_id = questionOption.question_id;
                existingOption.option = questionOption.option;
                existingOption.IsCorrect = questionOption.IsCorrect;
                existingOption.updated_at = DateTime.Now;
                existingOption.updated_by = questionOption.updated_by;

                _context.QuestionOption.Update(existingOption);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating QuestionOption: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the QuestionOption. Please try again later.");
            }
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionOption(int id)
        {
            try
            {
                var option = await _context.QuestionOption.FindAsync(id);
                if (option == null)
                {
                    return NotFound();
                }

                option.deleted_at = DateTime.Now;

                _context.QuestionOption.Update(option);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting QuestionOption: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the QuestionOption. Please try again later.");
            }
        }
    }
}

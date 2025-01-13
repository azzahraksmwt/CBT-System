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
    public class QuestionTopicController : ControllerBase
    {
        private readonly CBTContext _context;

        public QuestionTopicController(CBTContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateQuestionTopic([FromBody] QuestionTopic questionTopic)
        {
            try
            {
                if (questionTopic == null)
                {
                    return BadRequest("QuestionTopic cannot be null");
                }

                questionTopic.created_at = DateTime.Now;
                questionTopic.updated_at = DateTime.Now;

                _context.QuestionTopic.Add(questionTopic);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQuestionTopicById), new { id = questionTopic.id }, questionTopic);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating QuestionTopic: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the QuestionTopic. Please try again later.");
            }
        }

        // Get All
        [HttpGet]
        public async Task<ActionResult<IEnumerable<QuestionTopic>>> GetAllQuestionTopics()
        {
            try
            {
                var topics = await _context.QuestionTopic
                    .Where(qt => qt.deleted_at == null)
                    .ToListAsync();
                return Ok(topics);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving QuestionTopics: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the QuestionTopics. Please try again later.");
            }
        }

        // Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionTopic>> GetQuestionTopicById(int id)
        {
            try
            {
                var topic = await _context.QuestionTopic
                    .Include(t => t.questions.Where(t => t.deleted_at == null)) 
                    .ThenInclude(q => q.question_options.Where(q => q.deleted_at == null)) 
                    .FirstOrDefaultAsync(t => t.id == id);

                if (topic == null)
                {
                    return NotFound();
                }

                return Ok(topic);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving QuestionTopic by ID: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the QuestionTopic. Please try again later.");
            }
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestionTopic(int id, [FromBody] QuestionTopic questionTopic)
        {
            try
            {
                if (questionTopic == null || id != questionTopic.id)
                {
                    return BadRequest("Invalid QuestionTopic or ID mismatch");
                }

                var existingTopic = await _context.QuestionTopic.FindAsync(id);
                if (existingTopic == null)
                {
                    return NotFound();
                }

                existingTopic.topic_name = questionTopic.topic_name;
                existingTopic.difficulty_level = questionTopic.difficulty_level;
                existingTopic.duration = questionTopic.duration;
                existingTopic.total_questions = questionTopic.total_questions;
                existingTopic.updated_at = DateTime.Now;
                existingTopic.updated_by = questionTopic.updated_by;

                _context.QuestionTopic.Update(existingTopic);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating QuestionTopic: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the QuestionTopic. Please try again later.");
            }
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestionTopic(int id)
        {
            try
            {
                var topic = await _context.QuestionTopic.FindAsync(id);
                if (topic == null)
                {
                    return NotFound();
                }

                topic.deleted_at = DateTime.Now;

                _context.QuestionTopic.Update(topic);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting QuestionTopic: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the QuestionTopic. Please try again later.");
            }
        }
    }
}

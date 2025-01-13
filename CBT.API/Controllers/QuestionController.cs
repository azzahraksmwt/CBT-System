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
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly CBTContext _context;

        public QuestionController(CBTContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateQuestion([FromBody] Question question)
        {
            try
            {
                if (question == null)
                {
                    return BadRequest("Question cannot be null");
                }

                question.created_at = DateTime.Now;
                question.updated_at = DateTime.Now;

                _context.Question.Add(question);
                await _context.SaveChangesAsync();

                foreach (var option in question.question_options)
                {
                    option.question_id = question.id;
                    option.created_at = DateTime.Now;
                    option.updated_at = DateTime.Now;

                    if (option.id == 0)
                    {
                        _context.QuestionOption.Add(option);
                    }
                    else 
                    {
                        _context.QuestionOption.Update(option);
                    }
                }

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetQuestionById), new { id = question.id }, question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating question: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the question. Please try again later.");
            }
        }

        // Get All
        [HttpGet]
        //[Authorized]
        public async Task<ActionResult<IEnumerable<Question>>> GetAllQuestions()
        {
            try
            {
                var questions = await _context.Question
                    .Where(qt => qt.deleted_at == null)
                    .ToListAsync();
                return Ok(questions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving questions: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the questions. Please try again later.");
            }
        }

        // Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Question>> GetQuestionById(int id)
        {
            try
            {
                var question = await _context.Question
                    .Include(q => q.question_options)
                    .FirstOrDefaultAsync(x => x.id == id);

                if (question == null)
                {
                    return NotFound();
                }

                question.question_options = question.question_options
                    .Where(o => o.deleted_at == null)
                    .ToList();

                return Ok(question);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving question by ID: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the question. Please try again later.");
            }
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuestion(int id, [FromBody] Question question)
        {
            try
            {
                if (question == null || id != question.id)
                {
                    return BadRequest("Invalid question or ID mismatch");
                }

                var existingQuestion = await _context.Question
                    .Include(q => q.question_options)
                    .FirstOrDefaultAsync(q => q.id == id);

                if (existingQuestion == null)
                {
                    return NotFound();
                }

                existingQuestion.question = question.question;
                existingQuestion.weight = question.weight;
                existingQuestion.question_topic_id = question.question_topic_id;
                existingQuestion.created_at = question.created_at;
                existingQuestion.updated_at = DateTime.Now;
                existingQuestion.updated_by = question.updated_by;

                var newOptionIds = question.question_options.Select(o => o.id).ToList();
                foreach (var existingOption in existingQuestion.question_options)
                {
                    if (!newOptionIds.Contains(existingOption.id))
                    {
                        existingOption.deleted_at = DateTime.Now; 
                    }
                }

                foreach (var newOption in question.question_options)
                {
                    var existingOption = existingQuestion.question_options
                        .FirstOrDefault(o => o.id == newOption.id);

                    if (existingOption == null) 
                    {
                        existingQuestion.question_options.Add(new QuestionOption
                        {
                            option = newOption.option,
                            IsCorrect = newOption.IsCorrect,
                            question_id = existingQuestion.id,
                            created_at = DateTime.Now,
                            updated_at = DateTime.Now,
                        });
                    }
                    else 
                    {
                        existingOption.option = newOption.option;
                        existingOption.IsCorrect = newOption.IsCorrect;
                        existingOption.updated_at = DateTime.Now;
                        existingOption.deleted_at = null; 
                    }
                }

                _context.Question.Update(existingQuestion);

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating question: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the question. Please try again later.");
            }
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            try
            {
                var question = await _context.Question
                    .Include(q => q.question_options)
                    .FirstOrDefaultAsync(q => q.id == id);

                if (question == null)
                {
                    return NotFound();
                }

                question.deleted_at = DateTime.Now;

                foreach (var option in question.question_options)
                {
                    option.deleted_at = DateTime.Now;
                }

                _context.Question.Update(question);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting question: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the question. Please try again later.");
            }
        }
    }
}

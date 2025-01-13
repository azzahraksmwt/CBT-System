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
    public class ScheduleController : ControllerBase
    {
        private readonly CBTContext _context;

        public ScheduleController(CBTContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] Schedule schedule)
        {
            try
            {
                if (schedule == null)
                {
                    return BadRequest("Schedule cannot be null");
                }

                schedule.created_at = DateTime.Now;
                schedule.updated_at = DateTime.Now;

                _context.Schedule.Add(schedule);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetScheduleById), new { id = schedule.id }, schedule);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating Schedule: {ex.Message}");
                return StatusCode(500, "An error occurred while creating the Schedule. Please try again later.");
            }
        }

        // Get All
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Schedule>>> GetAllSchedules()
        {
            try
            {
                var schedules = await _context.Schedule
                    .Where(qt => qt.deleted_at == null)
                    .ToListAsync();
                return Ok(schedules);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Schedule: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the Schedule. Please try again later.");
            }
        }

        // Get By Id
        [HttpGet("{id}")]
        public async Task<ActionResult<Schedule>> GetScheduleById(int id)
        {
            try
            {
                var schedule = await _context.Schedule.FindAsync(id);
                if (schedule == null)
                {
                    return NotFound();
                }

                return Ok(schedule);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving Schedule by ID: {ex.Message}");
                return StatusCode(500, "An error occurred while retrieving the Schedule. Please try again later.");
            }
        }

        // Update
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] Schedule schedule)
        {
            try
            {
                if (schedule == null || id != schedule.id)
                {
                    return BadRequest("Invalid Schedule or ID mismatch");
                }

                var existingSchedule = await _context.Schedule.FindAsync(id);
                if (existingSchedule == null)
                {
                    return NotFound();
                }

                existingSchedule.question_topic_id = schedule.question_topic_id;
                existingSchedule.start_date = schedule.start_date;
                existingSchedule.end_date = schedule.end_date;
                existingSchedule.updated_at = DateTime.Now;
                existingSchedule.updated_by = schedule.updated_by;

                _context.Schedule.Update(existingSchedule);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating Schedule: {ex.Message}");
                return StatusCode(500, "An error occurred while updating the Schedule. Please try again later.");
            }
        }

        // Delete
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                var schedule = await _context.Schedule.FindAsync(id);
                if (schedule == null)
                {
                    return NotFound();
                }

                schedule.deleted_at = DateTime.Now;

                _context.Schedule.Update(schedule);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting Schedule: {ex.Message}");
                return StatusCode(500, "An error occurred while deleting the Schedule. Please try again later.");
            }
        }
    }
}

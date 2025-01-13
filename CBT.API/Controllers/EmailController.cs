using CBT.API.DbContext;
using CBT.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[ApiController]
[Route("api/[controller]")]
public class EmailController : ControllerBase
{
    private readonly EmailService _emailService;
    private readonly CBTContext _dbContext;
    private readonly IConfiguration _configuration;

    public EmailController(EmailService emailService, CBTContext dbContext, IConfiguration configuration)
    {
        _emailService = emailService;
        _dbContext = dbContext;
        _configuration = configuration;
    }

    [HttpPost("send-schedule-notification")]
    public async Task<IActionResult> SendScheduleNotification(int scheduleId)
    {
        var scheduleWithTopics = await (
            from s in _dbContext.Schedule
            join qt in _dbContext.QuestionTopic on s.question_topic_id equals qt.id
            where s.id == scheduleId
            select new
            {
                StartDate = s.start_date,
                EndDate = s.end_date,
                TopicName = qt.topic_name,
                Duration = qt.duration,
            }).ToListAsync();

        if (!scheduleWithTopics.Any())
        {
            return NotFound("Schedule not found.");
        }

        var startDate = scheduleWithTopics.First().StartDate;
        var endDate = scheduleWithTopics.First().EndDate;
        var topicName = scheduleWithTopics.Select(x => x.TopicName).Distinct().ToList();
        string topicList = string.Join(", ", topicName);
        var duration = scheduleWithTopics.First().Duration;

        var participants = await _dbContext.UserManagement
            .Where(u => u.role == "participant")
            .Select(u => u.email)
            .ToListAsync();

        if (!participants.Any())
        {
            return BadRequest("No participants found.");
        }

        var loginLink = "http://localhost:5151/";

        var emailModel = new EmailNotificationModel
        {
            Recipients = participants,
            TemplateId = 1,  
            StartDate = startDate,
            EndDate = endDate,
            TopicName = topicList,
            Duration = duration,
            LoginLink = loginLink,
        };

        try
        {
            await _emailService.SendEmailAsync(emailModel);

            return Ok("Emails sent successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending email: {ex.Message}");
        }
    }


    //[HttpPost("send-schedule-notification")]
    //public async Task<IActionResult> SendScheduleNotification(int scheduleId)
    //{
    //    var participants = await _dbContext.UserManagement
    //        .Where(u => u.role == "participant")
    //        .Select(u => u.email)
    //        .ToListAsync();

    //    if (!participants.Any())
    //    {
    //        return BadRequest("No participants found.");
    //    }

    //    var emailModel = new EmailNotificationModel
    //    {
    //        Recipients = participants,
    //        Subject = "New Schedule Created",
    //        Content = "<h1>Schedule Notification</h1><p>A new schedule has been created. Check the platform for details.</p>"
    //    };

    //    try
    //    {
    //        await _emailService.SendEmailAsync(emailModel);
    //        return Ok("Emails sent successfully.");
    //    }
    //    catch (Exception ex)
    //    {
    //        return StatusCode(500, $"Error sending email: {ex.Message}");
    //    }
    //}
}

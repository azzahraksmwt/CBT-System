using System.Net.Http.Headers;
using CBT.API.Model;
using Microsoft.Extensions.Configuration;

public class EmailService
{
    private readonly string _apiKey;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public EmailService(IConfiguration configuration)
    {
        _apiKey = configuration["Brevo:ApiKey"];
        _senderEmail = configuration["Brevo:SenderEmail"];
        _senderName = configuration["Brevo:SenderName"];

        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new Exception("Brevo API Key is missing in appsettings.json.");
        }
    }

    public async Task SendEmailAsync(EmailNotificationModel emailModel)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("api-key", _apiKey);

        var payload = new
        {
            sender = new { email = _senderEmail, name = _senderName },
            to = emailModel.Recipients.Select(email => new { email }).ToArray(),
            templateId = emailModel.TemplateId, 
            @params = new
            {
                startDate = emailModel.StartDate.ToString("dd-MM-yyyy"),
                endDate = emailModel.EndDate.ToString("dd-MM-yyyy"),
                topicName = emailModel.TopicName,
                duration = emailModel.Duration,
                loginLink = emailModel.LoginLink,
            }
        };

        try
        {
            var response = await client.PostAsJsonAsync("https://api.brevo.com/v3/smtp/email", payload);
            response.EnsureSuccessStatusCode();  
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Request failed: {ex.Message}");
            throw new Exception("Network error or invalid request.", ex);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw new Exception("An unexpected error occurred.", ex);
        }


        //if (!response.IsSuccessStatusCode)
        //{
        //    var error = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine($"Error response: {error}");
        //    throw new Exception($"Failed to send email: {error}");
        //}
    }
}

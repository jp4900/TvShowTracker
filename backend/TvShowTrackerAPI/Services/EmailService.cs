using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace TvShowTrackerAPI.Services;

/// <summary>
/// Email service implementation using MailKit
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Send recommendation email
    /// </summary>
    public async Task SendRecommendationEmailAsync(
        string toEmail,
        List<TvShowRecommendationDto> recommendations)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Email:FromName"] ?? "TV Show Tracker",
                _configuration["Email:FromAddress"] ?? "noreply@tvshowtracker.com"
            ));
            message.To.Add(new MailboxAddress("user", toEmail));
            message.Subject = "Your Personalized TV Show Recommendations";

            // Build HTML email body
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = BuildRecommendationEmailHtml(recommendations)
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);

            _logger.LogInformation("Recommendation email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send recommendation email to {Email}", toEmail);
            // Don't throw - email failures shouldn't break the app
        }
    }

    /// <summary>
    /// Send welcome email
    /// </summary>
    public async Task SendWelcomeEmailAsync(string toEmail)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _configuration["Email:FromName"] ?? "TV Show Tracker",
                _configuration["Email:FromAddress"] ?? "noreply@tvshowtracker.com"
            ));
            message.To.Add(new MailboxAddress("user", toEmail));
            message.Subject = "Welcome to TV Show Tracker! 🎬";

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
                    <html>
                    <body style='font-family: Arial, sans-serif;'>
                        <h2>Welcome! 🎉</h2>
                        <p>Thank you for joining TV Show Tracker!</p>
                        <p>Start exploring TV shows and add your favorites to get personalized recommendations.</p>
                        <p>Happy watching! 🍿</p>
                    </body>
                    </html>"
            };

            message.Body = bodyBuilder.ToMessageBody();

            await SendEmailAsync(message);

            _logger.LogInformation("Welcome email sent to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send welcome email to {Email}", toEmail);
        }
    }

    /// <summary>
    /// Test email connection
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var client = new SmtpClient();

            var host = _configuration["Email:SmtpHost"];
            var port = int.Parse(_configuration["Email:SmtpPort"] ?? "587");

            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);

            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password);
            }

            await client.DisconnectAsync(true);

            _logger.LogInformation("Email connection test successful");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email connection test failed");
            return false;
        }
    }

    /// <summary>
    /// Core method to send email via SMTP
    /// </summary>
    private async Task SendEmailAsync(MimeMessage message)
    {
        var host = _configuration["Email:SmtpHost"];
        var port = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
        var username = _configuration["Email:Username"];
        var password = _configuration["Email:Password"];

        if (string.IsNullOrEmpty(host))
        {
            _logger.LogWarning("SMTP host not configured. Email would be sent to: {To}", message.To);
            _logger.LogInformation("Email Subject: {Subject}", message.Subject);
            return;
        }

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMTP error sending email");
            throw;
        }
    }

    /// <summary>
    /// Build HTML email body for recommendations
    /// </summary>
    private string BuildRecommendationEmailHtml(List<TvShowRecommendationDto> recommendations)
    {
        var showsHtml = string.Join("", recommendations.Take(5).Select(r => $@"
            <div style='border: 1px solid #ddd; padding: 15px; margin: 10px 0; border-radius: 8px;'>
                <h3 style='margin: 0 0 10px 0;'>{r.Title}</h3>
                <p style='color: #666; margin: 5px 0;'>⭐ Rating: {r.Rating:F1}/10</p>
                <p style='color: #888; margin: 5px 0;'>📺 {string.Join(", ", r.Genres)}</p>
                <p style='color: #4CAF50; margin: 5px 0;'><strong>{r.Reason}</strong></p>
            </div>
        "));

        return $@"
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
                    .content {{ padding: 20px; background-color: #f9f9f9; }}
                    .footer {{ text-align: center; padding: 20px; color: #888; font-size: 12px; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>🎬 Your Personalized Recommendations</h1>
                    </div>
                    <div class='content'>
                        <p>Hi!</p>
                        <p>Based on your favorite shows, we've found some TV shows you might love:</p>
                        {showsHtml}
                        <p style='margin-top: 20px;'>
                            <a href='{_configuration["AppUrl"]}/recommendations' 
                               style='background-color: #4CAF50; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                                View All Recommendations
                            </a>
                        </p>
                    </div>
                    <div class='footer'>
                        <p>You're receiving this because you have favorites in TV Show Tracker.</p>
                        <p>© 2024 TV Show Tracker. All rights reserved.</p>
                    </div>
                </div>
            </body>
            </html>";
    }
}
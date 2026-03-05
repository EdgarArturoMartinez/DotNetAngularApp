using System.Net;
using System.Net.Mail;
using DotNetCoreWebApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DotNetCoreWebApi.Infrastructure.Services;

/// <summary>
/// Email service implementation using SMTP
/// Supports Gmail, Outlook, custom SMTP servers
/// </summary>
public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _smtpUsername;
    private readonly string _smtpPassword;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _frontendUrl;
    private readonly bool _enableSsl;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Load email settings from configuration
        var emailSettings = _configuration.GetSection("EmailSettings");
        _smtpHost = emailSettings["SmtpHost"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(emailSettings["SmtpPort"] ?? "587");
        _smtpUsername = emailSettings["SmtpUsername"] ?? "";
        _smtpPassword = emailSettings["SmtpPassword"] ?? "";
        _fromEmail = emailSettings["FromEmail"] ?? _smtpUsername;
        _fromName = emailSettings["FromName"] ?? "VeggyWorldShop";
        _frontendUrl = emailSettings["FrontendUrl"] ?? "http://localhost:4200";
        _enableSsl = bool.Parse(emailSettings["EnableSsl"] ?? "true");
    }

    /// <summary>
    /// Send password reset email with reset link
    /// </summary>
    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string customerName)
    {
        var resetLink = $"{_frontendUrl}/reset-password?token={Uri.EscapeDataString(resetToken)}&email={Uri.EscapeDataString(toEmail)}";
        
        var subject = "Reset Your Password - VeggyWorldShop";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .button {{ display: inline-block; padding: 15px 30px; background-color: #667eea; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 20px 0; }}
        .button:hover {{ background-color: #5568d3; }}
        .footer {{ background: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 10px 10px; }}
        .warning {{ background: #fff3cd; border: 1px solid #ffc107; padding: 15px; border-radius: 5px; margin: 20px 0; }}
        .token-box {{ background: white; border: 2px dashed #667eea; padding: 15px; margin: 20px 0; border-radius: 5px; font-family: monospace; word-break: break-all; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🥬 VeggyWorldShop</h1>
            <p>Password Reset Request</p>
        </div>
        <div class='content'>
            <p>Hello <strong>{customerName}</strong>,</p>
            
            <p>We received a request to reset your password for your VeggyWorldShop account.</p>
            
            <p>Click the button below to reset your password:</p>
            
            <div style='text-align: center;'>
                <a href='{resetLink}' class='button'>Reset Password</a>
            </div>
            
            <div class='warning'>
                <strong>⚠️ Important:</strong>
                <ul style='margin: 10px 0; padding-left: 20px;'>
                    <li>This link will expire in <strong>1 hour</strong></li>
                    <li>The link can only be used <strong>once</strong></li>
                    <li>If you didn't request this, please ignore this email</li>
                </ul>
            </div>
            
            <p>If the button doesn't work, copy and paste this link into your browser:</p>
            <div class='token-box'>{resetLink}</div>
            
            <p>If you didn't request a password reset, you can safely ignore this email. Your password will remain unchanged.</p>
            
            <p>Best regards,<br>The VeggyWorldShop Team</p>
        </div>
        <div class='footer'>
            <p>This is an automated email. Please do not reply.</p>
            <p>&copy; 2026 VeggyWorldShop. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        var plainTextBody = $@"
Hello {customerName},

We received a request to reset your password for your VeggyWorldShop account.

To reset your password, visit this link:
{resetLink}

IMPORTANT:
- This link will expire in 1 hour
- The link can only be used once
- If you didn't request this, please ignore this email

If you didn't request a password reset, you can safely ignore this email.

Best regards,
The VeggyWorldShop Team

---
This is an automated email. Please do not reply.
© 2026 VeggyWorldShop. All rights reserved.
";

        await SendEmailAsync(toEmail, subject, htmlBody, plainTextBody);
    }

    /// <summary>
    /// Send welcome email to new customers
    /// </summary>
    public async Task SendWelcomeEmailAsync(string toEmail, string customerName)
    {
        var subject = "Welcome to VeggyWorldShop! 🥬";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .button {{ display: inline-block; padding: 15px 30px; background-color: #4caf50; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 20px 0; }}
        .footer {{ background: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 10px 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🥬 Welcome to VeggyWorldShop!</h1>
        </div>
        <div class='content'>
            <p>Hello <strong>{customerName}</strong>,</p>
            
            <p>Thank you for joining VeggyWorldShop! We're excited to have you as part of our community.</p>
            
            <p>Start exploring our fresh, organic vegetables and plant-based products:</p>
            
            <div style='text-align: center;'>
                <a href='{_frontendUrl}' class='button'>Start Shopping</a>
            </div>
            
            <p>If you have any questions, feel free to contact our support team.</p>
            
            <p>Best regards,<br>The VeggyWorldShop Team</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 VeggyWorldShop. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, htmlBody, null);
    }

    /// <summary>
    /// Send email confirmation
    /// </summary>
    public async Task SendEmailConfirmationAsync(string toEmail, string confirmationToken, string customerName)
    {
        var confirmLink = $"{_frontendUrl}/confirm-email?token={Uri.EscapeDataString(confirmationToken)}&email={Uri.EscapeDataString(toEmail)}";
        
        var subject = "Confirm Your Email - VeggyWorldShop";
        var htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border: 1px solid #ddd; }}
        .button {{ display: inline-block; padding: 15px 30px; background-color: #667eea; color: white; text-decoration: none; border-radius: 5px; font-weight: bold; margin: 20px 0; }}
        .footer {{ background: #f1f1f1; padding: 20px; text-align: center; font-size: 12px; color: #666; border-radius: 0 0 10px 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🥬 VeggyWorldShop</h1>
            <p>Email Confirmation</p>
        </div>
        <div class='content'>
            <p>Hello <strong>{customerName}</strong>,</p>
            
            <p>Please confirm your email address by clicking the button below:</p>
            
            <div style='text-align: center;'>
                <a href='{confirmLink}' class='button'>Confirm Email</a>
            </div>
            
            <p>If you didn't create an account with VeggyWorldShop, you can safely ignore this email.</p>
            
            <p>Best regards,<br>The VeggyWorldShop Team</p>
        </div>
        <div class='footer'>
            <p>&copy; 2026 VeggyWorldShop. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

        await SendEmailAsync(toEmail, subject, htmlBody, null);
    }

    /// <summary>
    /// Core method to send emails via SMTP
    /// </summary>
    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody, string? plainTextBody)
    {
        try
        {
            // Check if email is configured
            if (string.IsNullOrEmpty(_smtpUsername) || string.IsNullOrEmpty(_smtpPassword))
            {
                _logger.LogWarning("Email sending is not configured. Email would be sent to: {Email}", toEmail);
                _logger.LogInformation("Email Subject: {Subject}", subject);
                _logger.LogInformation("Email Body (plain text): {Body}", plainTextBody ?? "N/A");
                
                // In development, just log instead of throwing
                if (_configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development")
                {
                    _logger.LogInformation("Development mode: Email logged instead of sent");
                    return;
                }
                
                throw new InvalidOperationException("Email service is not configured. Please add EmailSettings to appsettings.json");
            }

            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(new MailAddress(toEmail));
            message.Subject = subject;
            message.IsBodyHtml = true;
            message.Body = htmlBody;

            // Add plain text alternative if provided
            if (!string.IsNullOrEmpty(plainTextBody))
            {
                var plainView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, "text/plain");
                message.AlternateViews.Add(plainView);
            }

            using var smtpClient = new SmtpClient(_smtpHost, _smtpPort);
            smtpClient.EnableSsl = _enableSsl;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

            await smtpClient.SendMailAsync(message);
            
            _logger.LogInformation("Email sent successfully to: {Email}", toEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to: {Email}", toEmail);
            
            // In production, you might want to queue the email for retry
            // For now, we'll throw to let the caller handle it
            throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
        }
    }
}

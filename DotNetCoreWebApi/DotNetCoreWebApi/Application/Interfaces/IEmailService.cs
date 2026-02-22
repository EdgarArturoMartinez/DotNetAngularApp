namespace DotNetCoreWebApi.Application.Interfaces;

/// <summary>
/// Email service interface for sending emails
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Send password reset email with reset link
    /// </summary>
    /// <param name="toEmail">Recipient email</param>
    /// <param name="resetToken">Password reset token</param>
    /// <param name="customerName">Customer name for personalization</param>
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken, string customerName);

    /// <summary>
    /// Send welcome email to new customers
    /// </summary>
    Task SendWelcomeEmailAsync(string toEmail, string customerName);

    /// <summary>
    /// Send email verification
    /// </summary>
    Task SendEmailConfirmationAsync(string toEmail, string confirmationToken, string customerName);
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ViewStream.Application.Interfaces.Services
{
    public interface IEmailService
    {
        /// <summary>
        /// Send an email
        /// </summary>
        Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        
        /// <summary>
        /// Send email to multiple recipients
        /// </summary>
        Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true);
        
        /// <summary>
        /// Send email with attachments
        /// </summary>
        Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName);
        
        /// <summary>
        /// Send welcome email to new user
        /// </summary>
        Task SendWelcomeEmailAsync(string to, string userName, string confirmationLink);
        
        /// <summary>
        /// Send password reset email
        /// </summary>
        Task SendPasswordResetEmailAsync(string to, string userName, string resetLink);
        
        /// <summary>
        /// Send email verification email
        /// </summary>
        Task SendEmailVerificationAsync(string to, string userName, string verificationLink);
        
        /// <summary>
        /// Send notification email
        /// </summary>
        Task SendNotificationAsync(string to, string title, string message);
        
        /// <summary>
        /// Test email configuration
        /// </summary>
        Task TestEmailConfigurationAsync(string testEmail);
    }
}

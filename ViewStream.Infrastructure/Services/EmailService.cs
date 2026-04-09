using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ViewStream.Application.Interfaces.Services;
using ViewStream.Shared.Options;

namespace ViewStream.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
        {
            _emailOptions = emailOptions.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            await SendEmailAsync(new[] { to }, subject, body, isHtml);
        }

        public async Task SendEmailAsync(IEnumerable<string> to, string subject, string body, bool isHtml = true)
        {
            using var client = CreateSmtpClient();
            using var message = CreateMailMessage(to, subject, body, isHtml);
            
            try
            {
                await client.SendMailAsync(message);
                
                if (_emailOptions.EnableEmailLogging)
                {
                    _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", to));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", to));
                throw;
            }
        }

        public async Task SendEmailWithAttachmentAsync(string to, string subject, string body, byte[] attachment, string attachmentName)
        {
            using var client = CreateSmtpClient();
            using var message = CreateMailMessage(new[] { to }, subject, body, true);
            
            // Add attachment
            using var stream = new System.IO.MemoryStream(attachment);
            var attachmentFile = new Attachment(stream, attachmentName);
            message.Attachments.Add(attachmentFile);
            
            try
            {
                await client.SendMailAsync(message);
                
                if (_emailOptions.EnableEmailLogging)
                {
                    _logger.LogInformation("Email with attachment sent to {Recipient}", to);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email with attachment to {Recipient}", to);
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string to, string userName, string confirmationLink)
        {
            var subject = _emailOptions.WelcomeEmailSubject;
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ padding: 20px; background-color: #f4f4f4; }}
                        .content {{ background-color: white; padding: 20px; border-radius: 5px; }}
                        .button {{ background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='content'>
                            <h2>Welcome to ViewStream!</h2>
                            <p>Hello {userName},</p>
                            <p>Thank you for joining ViewStream. We're excited to have you on board!</p>
                            <p>Please confirm your email address by clicking the button below:</p>
                            <p><a href='{confirmationLink}' class='button'>Confirm Email</a></p>
                            <p>If the button doesn't work, copy and paste this link into your browser:</p>
                            <p>{confirmationLink}</p>
                            <br/>
                            <p>Best regards,<br/>The ViewStream Team</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            
            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendPasswordResetEmailAsync(string to, string userName, string resetLink)
        {
            var subject = _emailOptions.ResetPasswordSubject;
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ padding: 20px; background-color: #f4f4f4; }}
                        .content {{ background-color: white; padding: 20px; border-radius: 5px; }}
                        .button {{ background-color: #ff5722; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='content'>
                            <h2>Password Reset Request</h2>
                            <p>Hello {userName},</p>
                            <p>We received a request to reset your password. Click the button below to create a new password:</p>
                            <p><a href='{resetLink}' class='button'>Reset Password</a></p>
                            <p>If the button doesn't work, copy and paste this link into your browser:</p>
                            <p>{resetLink}</p>
                            <p>This link will expire in 1 hour.</p>
                            <p>If you didn't request this, please ignore this email.</p>
                            <br/>
                            <p>Best regards,<br/>The ViewStream Team</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            
            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendEmailVerificationAsync(string to, string userName, string verificationLink)
        {
            var subject = _emailOptions.VerificationEmailSubject;
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ padding: 20px; background-color: #f4f4f4; }}
                        .content {{ background-color: white; padding: 20px; border-radius: 5px; }}
                        .button {{ background-color: #2196F3; color: white; padding: 10px 20px; text-decoration: none; border-radius: 4px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='content'>
                            <h2>Verify Your Email Address</h2>
                            <p>Hello {userName},</p>
                            <p>Please verify your email address by clicking the button below:</p>
                            <p><a href='{verificationLink}' class='button'>Verify Email</a></p>
                            <p>If the button doesn't work, copy and paste this link into your browser:</p>
                            <p>{verificationLink}</p>
                            <br/>
                            <p>Best regards,<br/>The ViewStream Team</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            
            await SendEmailAsync(to, subject, body, true);
        }

        public async Task SendNotificationAsync(string to, string title, string message)
        {
            var subject = title;
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; }}
                        .container {{ padding: 20px; background-color: #f4f4f4; }}
                        .content {{ background-color: white; padding: 20px; border-radius: 5px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='content'>
                            <h2>{title}</h2>
                            <p>{message}</p>
                            <br/>
                            <p>Best regards,<br/>The ViewStream Team</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            
            await SendEmailAsync(to, subject, body, true);
        }

        public async Task TestEmailConfigurationAsync(string testEmail)
        {
            var subject = "Email Configuration Test";
            var body = @"
                <html>
                <head>
                    <style>
                        body { font-family: Arial, sans-serif; }
                        .container { padding: 20px; background-color: #f4f4f4; }
                        .content { background-color: white; padding: 20px; border-radius: 5px; }
                        .success { color: green; }
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='content'>
                            <h2 class='success'>Email Configuration Test Successful!</h2>
                            <p>This email confirms that your email settings are configured correctly.</p>
                            <p>Here are your current settings:</p>
                            <ul>
                                <li><strong>SMTP Server:</strong> " + _emailOptions.SmtpServer + @"</li>
                                <li><strong>SMTP Port:</strong> " + _emailOptions.SmtpPort + @"</li>
                                <li><strong>From Email:</strong> " + _emailOptions.FromEmail + @"</li>
                                <li><strong>From Name:</strong> " + _emailOptions.FromName + @"</li>
                                <li><strong>SSL Enabled:</strong> " + _emailOptions.EnableSsl + @"</li>
                            </ul>
                            <br/>
                            <p>Best regards,<br/>The ViewStream Team</p>
                        </div>
                    </div>
                </body>
                </html>
            ";
            
            await SendEmailAsync(testEmail, subject, body, true);
        }

        #region Private Methods

        private SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient(_emailOptions.SmtpServer, _emailOptions.SmtpPort)
            {
                EnableSsl = _emailOptions.EnableSsl,
                UseDefaultCredentials = _emailOptions.UseDefaultCredentials,
                Timeout = _emailOptions.Timeout
            };

            if (!_emailOptions.UseDefaultCredentials && !string.IsNullOrEmpty(_emailOptions.SmtpUsername))
            {
                client.Credentials = new NetworkCredential(_emailOptions.SmtpUsername, _emailOptions.SmtpPassword);
            }

            return client;
        }

        private MailMessage CreateMailMessage(IEnumerable<string> to, string subject, string body, bool isHtml)
        {
            var message = new MailMessage
            {
                From = new MailAddress(_emailOptions.FromEmail, _emailOptions.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = isHtml
            };

            foreach (var recipient in to)
            {
                if (!string.IsNullOrWhiteSpace(recipient))
                {
                    message.To.Add(recipient);
                }
            }

            // Add BCC if specified
            if (!string.IsNullOrEmpty(_emailOptions.BccEmail))
            {
                message.Bcc.Add(_emailOptions.BccEmail);
            }

            // Add CC if specified
            if (!string.IsNullOrEmpty(_emailOptions.CcEmail))
            {
                message.CC.Add(_emailOptions.CcEmail);
            }

            // Add Reply-To if specified
            if (!string.IsNullOrEmpty(_emailOptions.ReplyToEmail))
            {
                message.ReplyToList.Add(_emailOptions.ReplyToEmail);
            }

            return message;
        }

        #endregion
    }
}

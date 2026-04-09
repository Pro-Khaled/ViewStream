using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewStream.Shared.Options
{
    public class EmailOptions
    {
        public const string SectionName = "Email";
        
        // SMTP Server Settings
        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public string SmtpUsername { get; set; } = string.Empty;
        public string SmtpPassword { get; set; } = string.Empty;
        
        // Email Settings
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public string ReplyToEmail { get; set; } = string.Empty;
        
        // Security Settings
        public bool EnableSsl { get; set; } = true;
        public bool UseDefaultCredentials { get; set; } = false;
        
        // Email Templates
        public string WelcomeEmailTemplate { get; set; } = "welcome-template.html";
        public string ResetPasswordTemplate { get; set; } = "reset-password-template.html";
        public string VerificationEmailTemplate { get; set; } = "verification-template.html";
        
        // Advanced Settings
        public int Timeout { get; set; } = 10000; // milliseconds
        public int MaxRetryCount { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 5;
        public bool EnableEmailLogging { get; set; } = true;
        
        // Default Subjects
        public string WelcomeEmailSubject { get; set; } = "Welcome to ViewStream";
        public string ResetPasswordSubject { get; set; } = "Reset Your Password";
        public string VerificationEmailSubject { get; set; } = "Verify Your Email Address";
        
        // BCC/CC Settings
        public string BccEmail { get; set; } = string.Empty;
        public string CcEmail { get; set; } = string.Empty;
    }
}

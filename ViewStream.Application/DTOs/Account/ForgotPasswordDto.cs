using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs.Account
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

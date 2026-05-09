using System.ComponentModel.DataAnnotations;

namespace ViewStream.Application.DTOs.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public long UserId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword))]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}

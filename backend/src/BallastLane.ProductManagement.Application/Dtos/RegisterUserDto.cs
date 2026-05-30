using System.ComponentModel.DataAnnotations;

namespace BallastLane.ProductManagement.Application.Dtos
{
    /// <summary>
    /// DTO for user registration.
    /// </summary>
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = default!;

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [MinLength(6)]
        public string Password { get; set; } = default!;
    }
}

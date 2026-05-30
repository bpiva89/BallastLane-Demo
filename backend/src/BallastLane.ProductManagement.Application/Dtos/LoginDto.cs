using System.ComponentModel.DataAnnotations;

namespace BallastLane.ProductManagement.Application.Dtos
{
    /// <summary>
    /// DTO for user login.
    /// </summary>
    public class LoginDto
    {
        [Required]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;
    }
}

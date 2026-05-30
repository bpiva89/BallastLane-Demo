namespace BallastLane.ProductManagement.Application.Dtos
{
    /// <summary>
    /// Auth token response.
    /// </summary>
    public class AuthResponseDto
    {
        public string Token { get; set; } = default!;

        public string Username { get; set; } = default!;

        public DateTime ExpiresAt { get; set; }
    }
}

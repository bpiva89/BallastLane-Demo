using BallastLane.ProductManagement.Application.Dtos;

namespace BallastLane.ProductManagement.Application.Interfaces
{
    /// <summary>
    /// IAuthService Interface.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<AuthResponseDto> RegisterAsync(RegisterUserDto dto);

        /// <summary>
        /// Login and return a JWT token.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<AuthResponseDto> LoginAsync(LoginDto dto);
    }
}

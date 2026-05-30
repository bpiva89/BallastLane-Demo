using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Domain.Common;

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
        public Task<Result<AuthResponseDto>> RegisterAsync(RegisterUserDto dto);

        /// <summary>
        /// Login and return a JWT token.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    }
}

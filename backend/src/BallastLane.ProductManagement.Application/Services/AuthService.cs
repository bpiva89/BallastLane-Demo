using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Application.Interfaces;
using BallastLane.ProductManagement.Domain.Common;
using BallastLane.ProductManagement.Domain.Entities;
using BallastLane.ProductManagement.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace BallastLane.ProductManagement.Application.Services
{
    /// <summary>
    /// AuthService Class.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="configuration"></param>
        public AuthService(IUserRepository repository, IConfiguration configuration)
        {
            _repository = repository;
            _configuration = configuration;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterUserDto dto)
        {
            if (dto is null)
                return Result.Failure<AuthResponseDto>("Registration data cannot be null.");

            var existing = await _repository.GetByUsernameAsync(dto.Username);

            if (existing is not null)
                return Result.Failure<AuthResponseDto>($"Username '{dto.Username}' is already taken.");

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };

            var created = await _repository.InsertAsync(user);

            return Result.Success(BuildAuthResponse(created));
        }

        /// <summary>
        /// Login and return a JWT token.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
        {
            if (dto is null)
                return Result.Failure<AuthResponseDto>("Login data cannot be null.");

            var user = await _repository.GetByUsernameAsync(dto.Username);

            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Result.Failure<AuthResponseDto>("Invalid username or password.");

            return Result.Success(BuildAuthResponse(user));
        }

        private AuthResponseDto BuildAuthResponse(User user)
        {
            var jwtSection = _configuration.GetSection("Jwt");
            var secret = jwtSection["Secret"]!;
            var issuer = jwtSection["Issuer"]!;
            var audience = jwtSection["Audience"]!;
            var expiresInMinutes = int.Parse(jwtSection["ExpiresInMinutes"]!);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiresInMinutes);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                ExpiresAt = expiresAt
            };
        }
    }
}

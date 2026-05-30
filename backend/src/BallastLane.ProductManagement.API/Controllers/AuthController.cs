using BallastLane.ProductManagement.Application.Dtos;
using BallastLane.ProductManagement.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BallastLane.ProductManagement.API.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="authService"></param>
        public AuthController(ILogger<AuthController> logger, IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("AuthController.Register Started username={Username}.", dto.Username);

            var result = await _authService.RegisterAsync(dto);

            if (result.IsFailure)
            {
                _logger.LogWarning("AuthController.Register Failed username={Username} reason={Reason}.", dto.Username, result.Error);
                return BadRequest(result.Error);
            }

            _logger.LogInformation("AuthController.Register Finished username={Username}.", dto.Username);

            return Ok(result.Value);
        }

        /// <summary>
        /// Login and retrieve a JWT token.
        /// </summary>
        /// <param name="dto"></param>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _logger.LogInformation("AuthController.Login Started username={Username}.", dto.Username);

            var result = await _authService.LoginAsync(dto);

            if (result.IsFailure)
            {
                _logger.LogWarning("AuthController.Login Failed username={Username} reason={Reason}.", dto.Username, result.Error);
                return Unauthorized(result.Error);
            }

            _logger.LogInformation("AuthController.Login Finished username={Username}.", dto.Username);

            return Ok(result.Value);
        }
    }
}

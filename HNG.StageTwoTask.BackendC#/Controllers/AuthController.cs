using HNG.StageTwoTask.BackendC_.Implementation.Interfaces;
using HNG.StageTwoTask.BackendC_.Models.Access;
using Microsoft.AspNetCore.Mvc;

namespace HNG.StageTwoTask.BackendC_.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
        {
            var response = await _authService.RegisterAsync(request);

            if (response.Status == "success")
            {
                return StatusCode(201, response.Data); 
            }
            else if (response.Status == "error")
            {
                return StatusCode(422, response); 
            }
            else
            {
                return StatusCode(422, response.Data); 
            }
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            var response = await _authService.LoginAsync(request);

            if (response.Status == "error")
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}

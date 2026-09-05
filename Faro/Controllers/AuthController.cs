using Faro.Models;
using Faro.Services;
using Microsoft.AspNetCore.Mvc;

namespace Faro.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginRequest request)
        {
            var resultado =
                await _authService.LoginAsync(request);

            if (resultado == null)
            {
                return Unauthorized(new
                {
                    message =
                        "Número de teléfono o contraseña incorrectos."
                });
            }

            return Ok(resultado);
        }
    }
}
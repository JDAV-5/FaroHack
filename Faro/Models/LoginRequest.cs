using System.ComponentModel.DataAnnotations;

namespace Faro.Models
{
    public class LoginRequest
    {
        [Required]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
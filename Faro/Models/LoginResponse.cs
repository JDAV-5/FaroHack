namespace Faro.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;

        public Guid UsuarioId { get; set; }

        public string? Nombre { get; set; }

        public string? Apellido { get; set; }

        public string Telefono { get; set; } = string.Empty;

        public string Rol { get; set; } = string.Empty;
    }
}
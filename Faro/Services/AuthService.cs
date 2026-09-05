using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Faro.DataAccess;
using Faro.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Faro.Services
{
    public class AuthService : IAuthService
    {
        private readonly FaroDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(
            FaroDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var telefono = request.Telefono.Trim();

            // PostgreSQL verifica directamente el hash creado con pgcrypto
            var usuario = await _context.Usuarios
                .FromSqlInterpolated($@"
                    SELECT *
                    FROM usuarios
                    WHERE telefono = {telefono}
                      AND password_hash = crypt({request.Password}, password_hash)
                      AND activo = TRUE
                    LIMIT 1
                ")
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (usuario == null)
            {
                return null;
            }

            // Buscar el rol del usuario
            var rol = "SIN_ROL";

            if (usuario.RolId.HasValue)
            {
                rol = await _context.Roles
                    .Where(r =>
                        r.Id == usuario.RolId.Value &&
                        r.Activo)
                    .Select(r => r.Nombre)
                    .FirstOrDefaultAsync()
                    ?? "SIN_ROL";
            }

            // Actualizar último acceso
            await _context.Usuarios
                .Where(u => u.Id == usuario.Id)
                .ExecuteUpdateAsync(setters =>
                    setters.SetProperty(
                        u => u.UltimoAcceso,
                        DateTimeOffset.UtcNow
                    )
                );

            var token = GenerateToken(usuario, rol);

            return new LoginResponse
            {
                Token = token,
                UsuarioId = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Telefono = usuario.Telefono,
                Rol = rol
            };
        }

        private string GenerateToken(
            Usuario usuario,
            string rol)
        {
            var key = _configuration["Jwt:Key"]
                ?? throw new Exception(
                    "Jwt:Key no está configurado."
                );

            var issuer = _configuration["Jwt:Issuer"];
            var audience = _configuration["Jwt:Audience"];

            var expireMinutes = int.Parse(
                _configuration["Jwt:ExpireMinutes"] ?? "60"
            );

            var claims = new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    usuario.Id.ToString()
                ),

                new Claim(
                    "telefono",
                    usuario.Telefono
                ),

                new Claim(
                    ClaimTypes.Name,
                    $"{usuario.Nombre} {usuario.Apellido}".Trim()
                ),

                new Claim(
                    ClaimTypes.Role,
                    rol
                )
            };

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)
                );

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256
                );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow
                    .AddMinutes(expireMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}
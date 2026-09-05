using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Faro.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("rol_id")]
        public long? RolId { get; set; }

        [Column("nombre")]
        public string? Nombre { get; set; }

        [Column("apellido")]
        public string? Apellido { get; set; }

        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("edad")]
        public int? Edad { get; set; }

        [Column("departamento")]
        public string? Departamento { get; set; }

        [Column("municipio")]
        public string? Municipio { get; set; }

        [Column("activo")]
        public bool Activo { get; set; }

        [Column("ultimo_acceso")]
        public DateTimeOffset? UltimoAcceso { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}
using Faro.Models;
using Microsoft.EntityFrameworkCore;

namespace Faro.DataAccess
{
    public class FaroDbContext : DbContext
    {
        public FaroDbContext(DbContextOptions<FaroDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();

        public DbSet<Rol> Roles => Set<Rol>();
    }
}
using Microsoft.EntityFrameworkCore;
using ApiCentralDocsWeb.Model;

namespace ApiCentralDocsWeb.Data
{
    public class AppDbContext : DbContext
    { 
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Documento> Documento { get; set; } 
        public DbSet<TipoDocumento> TipoDocumento { get; set; }
    }
}

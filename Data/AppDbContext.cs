using Microsoft.EntityFrameworkCore;
using ApiCentralDocsWeb.Model;

namespace ApiCentralDocsWeb.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<TipoDocumento> TiposDocumento { get; set; }
        public DbSet<Foto> Fotos { get; set; }
    }
}
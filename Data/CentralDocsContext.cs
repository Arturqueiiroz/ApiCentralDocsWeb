using ApiCentralDocsWeb.Model;
using Microsoft.EntityFrameworkCore;

namespace ApiCentralDocsWeb.Data
{
    public class CentralDocsContext : DbContext
    {
        public CentralDocsContext(DbContextOptions<CentralDocsContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}

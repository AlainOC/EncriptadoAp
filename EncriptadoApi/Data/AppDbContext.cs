using Microsoft.EntityFrameworkCore;
using EncriptadoApi.Models;

namespace EncriptadoApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Mensaje> Mensajes { get; set; }
    }
} 
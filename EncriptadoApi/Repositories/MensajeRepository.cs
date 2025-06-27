using EncriptadoApi.Models;
using EncriptadoApi.Data;
using Microsoft.EntityFrameworkCore;

namespace EncriptadoApi.Repositories
{
    public class MensajeRepository
    {
        private readonly AppDbContext _context;
        public MensajeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AgregarMensajeAsync(Mensaje mensaje)
        {
            _context.Mensajes.Add(mensaje);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Mensaje>> ObtenerMensajesPaginadosAsync(int pagina, int cantidad)
        {
            return await _context.Mensajes
                .OrderByDescending(m => m.FechaCreacion)
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToListAsync();
        }
    }
} 
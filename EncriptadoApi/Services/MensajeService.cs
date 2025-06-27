using EncriptadoApi.Models;
using EncriptadoApi.Repositories;

namespace EncriptadoApi.Services
{
    public class MensajeService
    {
        private readonly MensajeRepository _repositorio;
        private const int DesplazamientoCesar = 3;

        public MensajeService(MensajeRepository repositorio)
        {
            _repositorio = repositorio;
        }

        private string CifradoCesar(string texto, int desplazamiento)
        {
            var resultado = new System.Text.StringBuilder();
            foreach (char c in texto)
            {
                if (char.IsLetter(c))
                {
                    char offset = char.IsUpper(c) ? 'A' : 'a';
                    resultado.Append((char)(((c + desplazamiento - offset) % 26) + offset));
                }
                else
                {
                    resultado.Append(c);
                }
            }
            return resultado.ToString();
        }

        private string DescifradoCesar(string texto, int desplazamiento)
        {
            return CifradoCesar(texto, 26 - (desplazamiento % 26));
        }

        public async Task<string> EncriptarAsync(string textoPlano)
        {
            var textoEncriptado = CifradoCesar(textoPlano, DesplazamientoCesar);
            var mensaje = new Mensaje
            {
                TextoEncriptado = textoEncriptado,
                TextoDesencriptado = textoPlano,
                FechaCreacion = DateTime.UtcNow
            };
            await _repositorio.AgregarMensajeAsync(mensaje);
            return textoEncriptado;
        }

        public async Task<string> DesencriptarAsync(string textoEncriptado)
        {
            var textoDesencriptado = DescifradoCesar(textoEncriptado, DesplazamientoCesar);
            var mensaje = new Mensaje
            {
                TextoEncriptado = textoEncriptado,
                TextoDesencriptado = textoDesencriptado,
                FechaCreacion = DateTime.UtcNow
            };
            await _repositorio.AgregarMensajeAsync(mensaje);
            return textoDesencriptado;
        }

        public async Task<List<Mensaje>> ObtenerHistorialAsync(int pagina, int cantidad)
        {
            return await _repositorio.ObtenerMensajesPaginadosAsync(pagina, cantidad);
        }
    }
} 
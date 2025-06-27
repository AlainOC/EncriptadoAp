using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using EncriptadoApi.Services;
using EncriptadoApi.Models;

namespace EncriptadoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EncriptadoController : ControllerBase
    {
        private readonly MensajeService _servicio;

        public EncriptadoController(MensajeService servicio)
        {
            _servicio = servicio;
        }

        [HttpPost("encriptar")]
        public async Task<ActionResult<string>> Encriptar([FromBody] string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
                return BadRequest("El texto no puede estar vacío.");

            var resultado = await _servicio.EncriptarAsync(textoPlano);
            return Ok(resultado);
        }

        [HttpPost("desencriptar")]
        public async Task<ActionResult<string>> Desencriptar([FromBody] string textoEncriptado)
        {
            if (string.IsNullOrEmpty(textoEncriptado))
                return BadRequest("El texto no puede estar vacío.");

            var resultado = await _servicio.DesencriptarAsync(textoEncriptado);
            return Ok(resultado);
        }

        [HttpGet("historial")]
        public async Task<ActionResult<IEnumerable<Mensaje>>> GetHistorial([FromQuery] int pagina = 1, [FromQuery] int cantidad = 10)
        {
            if (pagina < 1) pagina = 1;
            if (cantidad < 1) cantidad = 10;

            var mensajes = await _servicio.ObtenerHistorialAsync(pagina, cantidad);
            return Ok(mensajes);
        }
    }
} 
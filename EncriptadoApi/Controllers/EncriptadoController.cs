using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace EncriptadoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EncriptadoController : ControllerBase
    {
        private static readonly string Clave = "1234567890abcdef"; // 16 caracteres para AES-128
        private static readonly string IV = "abcdef1234567890";   // 16 caracteres para AES

        [HttpPost("encriptar")]
        public ActionResult<string> Encriptar([FromBody] string textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
                return BadRequest("El texto no puede estar vacío.");

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Clave);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(textoPlano);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            var resultado = System.Convert.ToBase64String(encryptedBytes);
            return Ok(resultado);
        }

        [HttpPost("desencriptar")]
        public ActionResult<string> Desencriptar([FromBody] string textoEncriptado)
        {
            if (string.IsNullOrEmpty(textoEncriptado))
                return BadRequest("El texto no puede estar vacío.");

            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Clave);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var encryptedBytes = System.Convert.FromBase64String(textoEncriptado);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            var resultado = Encoding.UTF8.GetString(decryptedBytes);
            return Ok(resultado);
        }
    }
} 
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EncriptadoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpPost("token")]
        public IActionResult GetToken([FromBody] LoginRequest request)
        {
            // Usuario y contraseña fijos para ejemplo
            if (request.Usuario != "admin" || request.Contrasena != "1234")
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.Usuario)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_clave_jwt_12345_1234567890_segura"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "encriptadoapi",
                audience: "encriptadoapi_usuarios",
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );
            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }
    }

    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
    }
} 
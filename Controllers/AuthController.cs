
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SimuladoIronFit.Api.DTOs;
using SimuladoIronFit.Api.Security;

namespace SimuladoIronFit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdminCredenciais _admin;
        private readonly IPasswordHasher<AdminCredenciais> _passwordHasher;

        public AuthController(
            IConfiguration configuration,
            AdminCredenciais admin,
            IPasswordHasher<AdminCredenciais> passwordHasher)
        {
            _configuration = configuration;
            _admin = admin;
            _passwordHasher = passwordHasher;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            bool emailValido = string.Equals(
                dto.Email.Trim(),
                _admin.Email,
                StringComparison.OrdinalIgnoreCase
            );

            bool senhaValida =
                _passwordHasher.VerifyHashedPassword(
                    _admin,
                    _admin.PasswordHash,
                    dto.Senha
                ) != PasswordVerificationResult.Failed;

            if (!emailValido || !senhaValida)
            {
                return Unauthorized(new
                {
                    mensagem = "E-mail ou senha inválidos."
                });
            }

            string jwtKey = _configuration["Jwt:Key"]!;
            string issuer = _configuration["Jwt:Issuer"]!;
            string audience = _configuration["Jwt:Audience"]!;

            int minutos = _configuration.GetValue<int>(
                "Jwt:ExpiresMinutes", 60
            );

            DateTime expiracao = DateTime.UtcNow.AddMinutes(minutos);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "academia"),

                new Claim(JwtRegisteredClaimNames.Email, _admin.Email),

                new Claim(ClaimTypes.Name, _admin.DisplayName),

                new Claim(ClaimTypes.Role, "AcademiaAdmin")
            };

            var chave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var credenciais = new SigningCredentials(
                chave,
                SecurityAlgorithms.HmacSha256
            );

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiracao,
                signingCredentials: credenciais
            );

            return Ok(new LoginRespostaDto
            {
                Token = new JwtSecurityTokenHandler()
                    .WriteToken(token),

                ExpiraEm = expiracao,

                Nome = _admin.DisplayName,

                Email = _admin.Email
            });
        }
    }
}


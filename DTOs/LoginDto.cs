
using System.ComponentModel.DataAnnotations;

namespace SimuladoIronFit.Api.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Senha { get; set; } = string.Empty;
    }
}


namespace SimuladoIronFit.Api.DTOs
{
    public class LoginRespostaDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime ExpiraEm { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
}


namespace SimuladoIronFit.Api.Models
{
    public class Alunos
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string Email { get; set; }

        public string Telefone { get; set; }

        public DateTime DataNascimento { get; set; }

        public int PersonalId { get; set; }

        public Personal Personal { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SimuladoIronFit.Api.Models
{
    public class Personal
    {
        public int Id { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string Cpf { get; set; }

        public ICollection<Alunos> Alunos { get; set; } = new List<Alunos>();
    }
}
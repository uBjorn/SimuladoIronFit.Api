using System.ComponentModel.DataAnnotations;

namespace SimuladoIronFit.Api.Models
{
    public class Agendamento
    {
        public int Id { get; set; }

        [Required]
        public DateTime DataAgendamento { get; set; }

        [Required]
        public int AlunoId { get; set; }

        public Alunos Aluno { get; set; }
    }
}
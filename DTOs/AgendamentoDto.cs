namespace SimuladoIronFit.Api.DTOs
{
    public class AgendamentoDto
    {
        public int Id { get; set; }

        public DateTime DataAgendamento { get; set; }

        public int AlunoId { get; set; }

        public string AlunoNome { get; set; }

        public string PersonalNome { get; set; }
    }
}
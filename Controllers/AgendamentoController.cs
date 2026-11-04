using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimuladoIronFit.Api.Data;
using SimuladoIronFit.Api.Models;
using SimuladoIronFit.Api.DTOs;

namespace SimuladoIronFit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgendamentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AgendamentoController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR AGENDAMENTOS
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var agendamentos = await _context.Agendamentos
                .Include(a => a.Aluno)
                .ThenInclude(a => a.Personal)
                .OrderBy(a => a.DataAgendamento)
                .Select(a => new AgendamentoDto
                {
                    Id = a.Id,
                    DataAgendamento = a.DataAgendamento,
                    AlunoId = a.AlunoId,
                    AlunoNome = a.Aluno.Nome,
                    PersonalNome = a.Aluno.Personal.Nome
                })
                .ToListAsync();

            return Ok(agendamentos);
        }

        // BUSCAR AGENDAMENTO POR ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var agendamento = await _context.Agendamentos
                .Include(a => a.Aluno)
                .ThenInclude(a => a.Personal)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (agendamento == null)
            {
                return NotFound("Agendamento não encontrado.");
            }

            var dto = new AgendamentoDto
            {
                Id = agendamento.Id,
                DataAgendamento = agendamento.DataAgendamento,
                AlunoId = agendamento.AlunoId,
                AlunoNome = agendamento.Aluno.Nome,
                PersonalNome = agendamento.Aluno.Personal.Nome
            };

            return Ok(dto);
        }

        // CADASTRAR AGENDAMENTO
        [HttpPost]
        public async Task<IActionResult> Cadastrar(AgendamentoDto dto)
        {
            var aluno = await _context.Alunos
                .FindAsync(dto.AlunoId);

            if (aluno == null)
            {
                return BadRequest("Aluno não encontrado.");
            }

            var agendamento = new Agendamento
            {
                DataAgendamento = dto.DataAgendamento,
                AlunoId = dto.AlunoId
            };

            _context.Agendamentos.Add(agendamento);

            await _context.SaveChangesAsync();

            return Ok("Agendamento cadastrado com sucesso!");
        }

        // ALTERAR DATA DO AGENDAMENTO
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, AgendamentoDto dto)
        {
            var agendamento = await _context.Agendamentos
                .FindAsync(id);

            if (agendamento == null)
            {
                return NotFound("Agendamento não encontrado.");
            }

            agendamento.DataAgendamento = dto.DataAgendamento;

            await _context.SaveChangesAsync();

            return Ok("Agendamento atualizado com sucesso!");
        }

        // EXCLUIR AGENDAMENTO
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var agendamento = await _context.Agendamentos
                .FindAsync(id);

            if (agendamento == null)
            {
                return NotFound("Agendamento não encontrado.");
            }

            _context.Agendamentos.Remove(agendamento);

            await _context.SaveChangesAsync();

            return Ok("Agendamento excluído com sucesso!");
        }
    }
}
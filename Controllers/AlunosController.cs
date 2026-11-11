
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimuladoIronFit.Api.Data;
using SimuladoIronFit.Api.Models;
using SimuladoIronFit.Api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace SimuladoIronFit.Api.Controllers
{
    [Authorize(Roles = "AcademiaAdmin")]
    [ApiController]
    [Route("api/[controller]")]
    public class AlunosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AlunosController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR ALUNOS
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var alunos = await _context.Alunos
                .Include(a => a.Personal)
                .Select(a => new AlunosDto
                {
                    Id = a.Id,
                    Nome = a.Nome,
                    Email = a.Email,
                    Telefone = a.Telefone,
                    DataNascimento = a.DataNascimento,
                    PersonalId = a.PersonalId,
                    PersonalNome = a.Personal.Nome
                })
                .ToListAsync();

            return Ok(alunos);
        }

        // BUSCAR ALUNO POR ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Buscar(int id)
        {
            var aluno = await _context.Alunos
                .Include(a => a.Personal)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (aluno == null)
            {
                return NotFound("Aluno não encontrado.");
            }

            var dto = new AlunosDto
            {
                Id = aluno.Id,
                Nome = aluno.Nome,
                Email = aluno.Email,
                Telefone = aluno.Telefone,
                DataNascimento = aluno.DataNascimento,
                PersonalId = aluno.PersonalId,
                PersonalNome = aluno.Personal.Nome
            };

            return Ok(dto);
        }

        // CADASTRAR ALUNO
        [HttpPost]
        public async Task<IActionResult> Cadastrar(AlunosDto dto)
        {
            var personal = await _context.Personals
                .FindAsync(dto.PersonalId);

            if (personal == null)
            {
                return BadRequest("Personal não encontrado.");
            }

            var aluno = new Alunos
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                DataNascimento = dto.DataNascimento,
                PersonalId = dto.PersonalId
            };

            _context.Alunos.Add(aluno);

            await _context.SaveChangesAsync();

            return Ok("Aluno cadastrado com sucesso!");
        }

        // EDITAR ALUNO
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, AlunosDto dto)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
            {
                return NotFound("Aluno não encontrado.");
            }

            var personal = await _context.Personals
                .FindAsync(dto.PersonalId);

            if (personal == null)
            {
                return BadRequest("Personal não encontrado.");
            }

            aluno.Nome = dto.Nome;
            aluno.Email = dto.Email;
            aluno.Telefone = dto.Telefone;
            aluno.DataNascimento = dto.DataNascimento;
            aluno.PersonalId = dto.PersonalId;

            await _context.SaveChangesAsync();

            return Ok("Aluno atualizado com sucesso!");
        }

        // EXCLUIR ALUNO
        [HttpDelete("{id}")]
        public async Task<IActionResult> Excluir(int id)
        {
            var aluno = await _context.Alunos.FindAsync(id);

            if (aluno == null)
            {
                return NotFound("Aluno não encontrado.");
            }

            _context.Alunos.Remove(aluno);

            await _context.SaveChangesAsync();

            return Ok("Aluno excluído com sucesso!");
        }
    }
}

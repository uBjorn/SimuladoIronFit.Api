using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimuladoIronFit.Api.Data;
using SimuladoIronFit.Api.DTOs;
using SimuladoIronFit.Api.Models;
using System.Security.Cryptography;
using System.Text;

namespace SimuladoIronFit.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonalController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDataProtector _protector;

        public PersonalController(AppDbContext context, IDataProtectionProvider provider)
        {
            _context = context;
            _protector = provider.CreateProtector("IronFit.CPF");
        }

        // CRIPTOGRAFAR CPF
        private string Criptografar(string cpf)
        {
            return _protector.Protect(cpf);
        }

        // LISTAR PERSONAL
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var personals = await _context.Personals
                .Select(p => new PersonalDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Cpf = "Criptografado"
                })
                .ToListAsync();

            return Ok(personals);
        }

        // PESQUISAR PERSONAL
        [HttpGet("buscar")]
        public async Task<IActionResult> Buscar(string nome)
        {
            var personals = await _context.Personals
                .Where(p => p.Nome.Contains(nome))
                .Select(p => new PersonalDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Cpf = "Criptografado"
                })
                .ToListAsync();

            return Ok(personals);
        }

        // CADASTRAR PERSONAL
        [HttpPost]
        public async Task<IActionResult> Cadastrar(PersonalDto dto)
        {
            var personal = new Personal
            {
                Nome = dto.Nome,
                Cpf = Criptografar(dto.Cpf)
            };

            _context.Personals.Add(personal);

            await _context.SaveChangesAsync();

            return Ok("Personal cadastrado com sucesso!");
        }

        // EDITAR PERSONAL
        [HttpPut("{id}")]
        public async Task<IActionResult> Editar(int id, PersonalDto dto)
        {
            var personal = await _context.Personals.FindAsync(id);

            if (personal == null)
            {
                return NotFound("Personal não encontrado.");
            }

            personal.Nome = dto.Nome;

            if (!string.IsNullOrWhiteSpace(dto.Cpf))
            {
                personal.Cpf = Criptografar(dto.Cpf);
            }

            await _context.SaveChangesAsync();

            return Ok("Personal atualizado com sucesso!");
        }
    }
}
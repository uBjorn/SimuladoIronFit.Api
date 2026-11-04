using Microsoft.EntityFrameworkCore;
using SimuladoIronFit.Api.Models;

namespace SimuladoIronFit.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Alunos> Alunos { get; set; }

        public DbSet<Personal> Personals { get; set; }

        public DbSet<Agendamento> Agendamentos { get; set; }
    }
}
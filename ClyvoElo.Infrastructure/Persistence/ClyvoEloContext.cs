using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Persistence;

public class ClyvoEloContext : DbContext
{
    public ClyvoEloContext(DbContextOptions<ClyvoEloContext> options) : base(options)
    {
    }

    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<Clinica> Clinicas => Set<Clinica>();
    public DbSet<Doutor> Doutores => Set<Doutor>();
    public DbSet<EventoClinico> EventosClinicos => Set<EventoClinico>();
    public DbSet<DispositivoIot> DispositivosIot => Set<DispositivoIot>();
    public DbSet<LeituraSensor> LeiturasSensor => Set<LeituraSensor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClyvoEloContext).Assembly);
    }
}

using Microsoft.EntityFrameworkCore;
using ClyvoElo.Api.Models;

namespace ClyvoElo.Api.Data;

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
        modelBuilder.Entity<Tutor>(entity =>
        {
            entity.ToTable("TUTOR");
            entity.HasKey(t => t.IdTutor);
            entity.Property(t => t.IdTutor).HasColumnName("ID_TUTOR");
            entity.Property(t => t.Nome).HasColumnName("NOME");
            entity.Property(t => t.Email).HasColumnName("EMAIL");
            entity.Property(t => t.SenhaHash).HasColumnName("SENHA_HASH");
            entity.Property(t => t.Telefone).HasColumnName("TELEFONE");
            entity.Property(t => t.DataCadastro).HasColumnName("DATA_CADASTRO");
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("PET");
            entity.HasKey(p => p.IdPet);
            entity.Property(p => p.IdPet).HasColumnName("ID_PET");
            entity.Property(p => p.IdTutor).HasColumnName("ID_TUTOR");
            entity.Property(p => p.Nome).HasColumnName("NOME");
            entity.Property(p => p.Especie).HasColumnName("ESPECIE");
            entity.Property(p => p.Raca).HasColumnName("RACA");
            entity.Property(p => p.DataNascimento).HasColumnName("DATA_NASCIMENTO");
            entity.Property(p => p.PesoKg).HasColumnName("PESO_KG");

            entity.HasOne(p => p.Tutor)
                  .WithMany(t => t.Pets)
                  .HasForeignKey(p => p.IdTutor);
        });

        modelBuilder.Entity<Clinica>(entity =>
        {
            entity.ToTable("CLINICA");
            entity.HasKey(c => c.IdClinica);
            entity.Property(c => c.IdClinica).HasColumnName("ID_CLINICA");
            entity.Property(c => c.Nome).HasColumnName("NOME");
            entity.Property(c => c.Telefone).HasColumnName("TELEFONE");
        });

        modelBuilder.Entity<Doutor>(entity =>
        {
            entity.ToTable("DOUTOR");
            entity.HasKey(d => d.IdDoutor);
            entity.Property(d => d.IdDoutor).HasColumnName("ID_DOUTOR");
            entity.Property(d => d.IdClinica).HasColumnName("ID_CLINICA");
            entity.Property(d => d.Nome).HasColumnName("NOME");
            entity.Property(d => d.Crmv).HasColumnName("CRMV");
            entity.Property(d => d.Email).HasColumnName("EMAIL");
            entity.Property(d => d.SenhaHash).HasColumnName("SENHA_HASH");

            entity.HasOne(d => d.Clinica)
                  .WithMany(c => c.Doutores)
                  .HasForeignKey(d => d.IdClinica);
        });

        modelBuilder.Entity<EventoClinico>(entity =>
        {
            entity.ToTable("EVENTO_CLINICO");
            entity.HasKey(e => e.IdEvento);
            entity.Property(e => e.IdEvento).HasColumnName("ID_EVENTO");
            entity.Property(e => e.IdPet).HasColumnName("ID_PET");
            entity.Property(e => e.IdDoutor).HasColumnName("ID_DOUTOR");
            entity.Property(e => e.TipoEvento).HasColumnName("TIPO_EVENTO");
            entity.Property(e => e.Descricao).HasColumnName("DESCRICAO");
            entity.Property(e => e.DataEvento).HasColumnName("DATA_EVENTO");
            entity.Property(e => e.Status).HasColumnName("STATUS");

            entity.HasOne(e => e.Pet)
                  .WithMany(p => p.EventosClinicos)
                  .HasForeignKey(e => e.IdPet);

            entity.HasOne(e => e.Doutor)
                  .WithMany()
                  .HasForeignKey(e => e.IdDoutor);
        });

        modelBuilder.Entity<DispositivoIot>(entity =>
        {
            entity.ToTable("DISPOSITIVO_IOT");
            entity.HasKey(d => d.IdDispositivo);
            entity.Property(d => d.IdDispositivo).HasColumnName("ID_DISPOSITIVO");
            entity.Property(d => d.IdPet).HasColumnName("ID_PET");
            entity.Property(d => d.TipoDispositivo).HasColumnName("TIPO_DISPOSITIVO");
            entity.Property(d => d.DataInstalacao).HasColumnName("DATA_INSTALACAO");

            entity.HasOne(d => d.Pet)
                  .WithMany(p => p.Dispositivos)
                  .HasForeignKey(d => d.IdPet);
        });

        modelBuilder.Entity<LeituraSensor>(entity =>
        {
            entity.ToTable("LEITURA_SENSOR");
            entity.HasKey(l => l.IdLeitura);
            entity.Property(l => l.IdLeitura).HasColumnName("ID_LEITURA");
            entity.Property(l => l.IdDispositivo).HasColumnName("ID_DISPOSITIVO");
            entity.Property(l => l.TipoMedida).HasColumnName("TIPO_MEDIDA");
            entity.Property(l => l.ValorMedido).HasColumnName("VALOR_MEDIDO");
            entity.Property(l => l.DataHoraLeitura).HasColumnName("DATA_HORA_LEITURA");

            entity.HasOne(l => l.Dispositivo)
                  .WithMany(d => d.Leituras)
                  .HasForeignKey(l => l.IdDispositivo);
        });
    }
}

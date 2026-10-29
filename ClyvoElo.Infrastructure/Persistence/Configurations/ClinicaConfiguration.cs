using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoElo.Infrastructure.Persistence.Configurations;

public class ClinicaConfiguration : IEntityTypeConfiguration<Clinica>
{
    public void Configure(EntityTypeBuilder<Clinica> builder)
    {
        builder.ToTable("CLINICA");
        builder.HasKey(c => c.IdClinica);

        builder.Property(c => c.IdClinica).HasColumnName("ID_CLINICA").ValueGeneratedOnAdd();
        builder.Property(c => c.Nome).HasColumnName("NOME").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(c => c.Telefone).HasColumnName("TELEFONE").HasMaxLength(20).IsUnicode(false);
    }
}

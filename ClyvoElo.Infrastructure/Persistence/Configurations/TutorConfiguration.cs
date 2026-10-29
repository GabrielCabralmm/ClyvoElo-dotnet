using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoElo.Infrastructure.Persistence.Configurations;

public class TutorConfiguration : IEntityTypeConfiguration<Tutor>
{
    public void Configure(EntityTypeBuilder<Tutor> builder)
    {
        builder.ToTable("TUTOR");
        builder.HasKey(t => t.IdTutor);

        builder.Property(t => t.IdTutor).HasColumnName("ID_TUTOR").ValueGeneratedOnAdd();
        builder.Property(t => t.Nome).HasColumnName("NOME").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(t => t.Email).HasColumnName("EMAIL").HasMaxLength(150).IsUnicode(false).IsRequired();
        builder.Property(t => t.SenhaHash).HasColumnName("SENHA_HASH").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(t => t.Telefone).HasColumnName("TELEFONE").HasMaxLength(20).IsUnicode(false);
        builder.Property(t => t.DataCadastro).HasColumnName("DATA_CADASTRO").HasColumnType("DATE");

        builder.HasIndex(t => t.Email).IsUnique();
    }
}

using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoElo.Infrastructure.Persistence.Configurations;

public class DoutorConfiguration : IEntityTypeConfiguration<Doutor>
{
    public void Configure(EntityTypeBuilder<Doutor> builder)
    {
        builder.ToTable("DOUTOR");
        builder.HasKey(d => d.IdDoutor);

        builder.Property(d => d.IdDoutor).HasColumnName("ID_DOUTOR").ValueGeneratedOnAdd();
        builder.Property(d => d.IdClinica).HasColumnName("ID_CLINICA");
        builder.Property(d => d.Nome).HasColumnName("NOME").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(d => d.Crmv).HasColumnName("CRMV").HasMaxLength(20).IsUnicode(false).IsRequired();
        builder.Property(d => d.Email).HasColumnName("EMAIL").HasMaxLength(150).IsUnicode(false).IsRequired();
        builder.Property(d => d.SenhaHash).HasColumnName("SENHA_HASH").HasMaxLength(100).IsUnicode(false).IsRequired();

        builder.HasIndex(d => d.Email).IsUnique();
        builder.HasIndex(d => d.Crmv).IsUnique();

        builder.HasOne(d => d.Clinica)
            .WithMany(c => c.Doutores)
            .HasForeignKey(d => d.IdClinica)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoElo.Infrastructure.Persistence.Configurations;

public class PetConfiguration : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("PET");
        builder.HasKey(p => p.IdPet);

        builder.Property(p => p.IdPet).HasColumnName("ID_PET").ValueGeneratedOnAdd();
        builder.Property(p => p.IdTutor).HasColumnName("ID_TUTOR");
        builder.Property(p => p.Nome).HasColumnName("NOME").HasMaxLength(100).IsUnicode(false).IsRequired();
        builder.Property(p => p.Especie).HasColumnName("ESPECIE").HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(p => p.Raca).HasColumnName("RACA").HasMaxLength(100).IsUnicode(false);
        builder.Property(p => p.DataNascimento).HasColumnName("DATA_NASCIMENTO").HasColumnType("DATE");
        builder.Property(p => p.PesoKg).HasColumnName("PESO_KG").HasPrecision(5, 2);

        builder.HasOne(p => p.Tutor)
            .WithMany(t => t.Pets)
            .HasForeignKey(p => p.IdTutor)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

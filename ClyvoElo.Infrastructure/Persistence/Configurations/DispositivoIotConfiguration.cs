using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoElo.Infrastructure.Persistence.Configurations;

public class DispositivoIotConfiguration : IEntityTypeConfiguration<DispositivoIot>
{
    public void Configure(EntityTypeBuilder<DispositivoIot> builder)
    {
        builder.ToTable("DISPOSITIVO_IOT");
        builder.HasKey(d => d.IdDispositivo);

        builder.Property(d => d.IdDispositivo).HasColumnName("ID_DISPOSITIVO").ValueGeneratedOnAdd();
        builder.Property(d => d.IdPet).HasColumnName("ID_PET");
        builder.Property(d => d.TipoDispositivo).HasColumnName("TIPO_DISPOSITIVO").HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(d => d.DataInstalacao).HasColumnName("DATA_INSTALACAO").HasColumnType("DATE");

        builder.HasOne(d => d.Pet)
            .WithMany(p => p.Dispositivos)
            .HasForeignKey(d => d.IdPet)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

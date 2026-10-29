using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoElo.Infrastructure.Persistence.Configurations;

public class LeituraSensorConfiguration : IEntityTypeConfiguration<LeituraSensor>
{
    public void Configure(EntityTypeBuilder<LeituraSensor> builder)
    {
        builder.ToTable("LEITURA_SENSOR");
        builder.HasKey(l => l.IdLeitura);

        builder.Property(l => l.IdLeitura).HasColumnName("ID_LEITURA").ValueGeneratedOnAdd();
        builder.Property(l => l.IdDispositivo).HasColumnName("ID_DISPOSITIVO");
        builder.Property(l => l.TipoMedida).HasColumnName("TIPO_MEDIDA").HasMaxLength(50).IsUnicode(false).IsRequired();
        builder.Property(l => l.ValorMedido).HasColumnName("VALOR_MEDIDO").HasPrecision(10, 2);
        builder.Property(l => l.DataHoraLeitura).HasColumnName("DATA_HORA_LEITURA").HasColumnType("TIMESTAMP");

        builder.HasOne(l => l.Dispositivo)
            .WithMany(d => d.Leituras)
            .HasForeignKey(l => l.IdDispositivo)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

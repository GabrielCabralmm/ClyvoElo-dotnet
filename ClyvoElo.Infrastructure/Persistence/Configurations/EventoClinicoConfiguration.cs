using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClyvoElo.Infrastructure.Persistence.Configurations;

public class EventoClinicoConfiguration : IEntityTypeConfiguration<EventoClinico>
{
    public void Configure(EntityTypeBuilder<EventoClinico> builder)
    {
        builder.ToTable("EVENTO_CLINICO", tabela =>
        {
            tabela.HasCheckConstraint("CK_EVENTO_TIPO", "TIPO_EVENTO IN ('VACINA','CHECKUP','POS_OPERATORIO','MEDICACAO','EXAME')");
            tabela.HasCheckConstraint("CK_EVENTO_STATUS", "STATUS IN ('PENDENTE','CONCLUIDO','ATRASADO')");
        });
        builder.HasKey(e => e.IdEvento);

        builder.Property(e => e.IdEvento).HasColumnName("ID_EVENTO").ValueGeneratedOnAdd();
        builder.Property(e => e.IdPet).HasColumnName("ID_PET");
        builder.Property(e => e.IdDoutor).HasColumnName("ID_DOUTOR");
        builder.Property(e => e.TipoEvento).HasColumnName("TIPO_EVENTO").HasMaxLength(30).IsUnicode(false).IsRequired();
        builder.Property(e => e.Descricao).HasColumnName("DESCRICAO").HasMaxLength(500).IsUnicode(false);
        builder.Property(e => e.DataEvento).HasColumnName("DATA_EVENTO").HasColumnType("DATE");
        builder.Property(e => e.Status).HasColumnName("STATUS").HasMaxLength(20).IsUnicode(false).IsRequired();

        builder.HasOne(e => e.Pet)
            .WithMany(p => p.EventosClinicos)
            .HasForeignKey(e => e.IdPet)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Doutor)
            .WithMany()
            .HasForeignKey(e => e.IdDoutor)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

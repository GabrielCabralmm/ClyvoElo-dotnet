using System;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;

#nullable disable

namespace ClyvoElo.Infrastructure.Persistence.Migrations
{
    [DbContext(typeof(ClyvoEloContext))]
    partial class ClyvoEloContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "10.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Clinica", b =>
                {
                    b.Property<int>("IdClinica")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_CLINICA");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdClinica"));

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(100)")
                        .HasColumnName("NOME");

                    b.Property<string>("Telefone")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(20)")
                        .HasColumnName("TELEFONE");

                    b.HasKey("IdClinica");

                    b.ToTable("CLINICA", (string)null);
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.DispositivoIot", b =>
                {
                    b.Property<int>("IdDispositivo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_DISPOSITIVO");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDispositivo"));

                    b.Property<DateTime>("DataInstalacao")
                        .HasColumnType("DATE")
                        .HasColumnName("DATA_INSTALACAO");

                    b.Property<int>("IdPet")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_PET");

                    b.Property<string>("TipoDispositivo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(50)")
                        .HasColumnName("TIPO_DISPOSITIVO");

                    b.HasKey("IdDispositivo");

                    b.HasIndex("IdPet");

                    b.ToTable("DISPOSITIVO_IOT", (string)null);
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Doutor", b =>
                {
                    b.Property<int>("IdDoutor")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_DOUTOR");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDoutor"));

                    b.Property<string>("Crmv")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(20)")
                        .HasColumnName("CRMV");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(150)")
                        .HasColumnName("EMAIL");

                    b.Property<int>("IdClinica")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_CLINICA");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(100)")
                        .HasColumnName("NOME");

                    b.Property<string>("SenhaHash")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(100)")
                        .HasColumnName("SENHA_HASH");

                    b.HasKey("IdDoutor");

                    b.HasIndex("Crmv")
                        .IsUnique();

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("IdClinica");

                    b.ToTable("DOUTOR", (string)null);
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.EventoClinico", b =>
                {
                    b.Property<int>("IdEvento")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_EVENTO");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdEvento"));

                    b.Property<DateTime>("DataEvento")
                        .HasColumnType("DATE")
                        .HasColumnName("DATA_EVENTO");

                    b.Property<string>("Descricao")
                        .HasMaxLength(500)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(500)")
                        .HasColumnName("DESCRICAO");

                    b.Property<int>("IdDoutor")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_DOUTOR");

                    b.Property<int>("IdPet")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_PET");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(20)")
                        .HasColumnName("STATUS");

                    b.Property<string>("TipoEvento")
                        .IsRequired()
                        .HasMaxLength(30)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(30)")
                        .HasColumnName("TIPO_EVENTO");

                    b.HasKey("IdEvento");

                    b.HasIndex("IdDoutor");

                    b.HasIndex("IdPet");

                    b.ToTable("EVENTO_CLINICO", null, t =>
                        {
                            t.HasCheckConstraint("CK_EVENTO_STATUS", "STATUS IN ('PENDENTE','CONCLUIDO','ATRASADO')");

                            t.HasCheckConstraint("CK_EVENTO_TIPO", "TIPO_EVENTO IN ('VACINA','CHECKUP','POS_OPERATORIO','MEDICACAO','EXAME')");
                        });
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.LeituraSensor", b =>
                {
                    b.Property<int>("IdLeitura")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_LEITURA");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdLeitura"));

                    b.Property<DateTime>("DataHoraLeitura")
                        .HasColumnType("TIMESTAMP")
                        .HasColumnName("DATA_HORA_LEITURA");

                    b.Property<int>("IdDispositivo")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_DISPOSITIVO");

                    b.Property<string>("TipoMedida")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(50)")
                        .HasColumnName("TIPO_MEDIDA");

                    b.Property<decimal>("ValorMedido")
                        .HasPrecision(10, 2)
                        .HasColumnType("DECIMAL(10,2)")
                        .HasColumnName("VALOR_MEDIDO");

                    b.HasKey("IdLeitura");

                    b.HasIndex("IdDispositivo");

                    b.ToTable("LEITURA_SENSOR", (string)null);
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Pet", b =>
                {
                    b.Property<int>("IdPet")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_PET");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPet"));

                    b.Property<DateTime?>("DataNascimento")
                        .HasColumnType("DATE")
                        .HasColumnName("DATA_NASCIMENTO");

                    b.Property<string>("Especie")
                        .IsRequired()
                        .HasMaxLength(50)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(50)")
                        .HasColumnName("ESPECIE");

                    b.Property<int>("IdTutor")
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_TUTOR");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(100)")
                        .HasColumnName("NOME");

                    b.Property<decimal?>("PesoKg")
                        .HasPrecision(5, 2)
                        .HasColumnType("DECIMAL(5,2)")
                        .HasColumnName("PESO_KG");

                    b.Property<string>("Raca")
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(100)")
                        .HasColumnName("RACA");

                    b.HasKey("IdPet");

                    b.HasIndex("IdTutor");

                    b.ToTable("PET", (string)null);
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Tutor", b =>
                {
                    b.Property<int>("IdTutor")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NUMBER(10)")
                        .HasColumnName("ID_TUTOR");

                    OraclePropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdTutor"));

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("DATE")
                        .HasColumnName("DATA_CADASTRO");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(150)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(150)")
                        .HasColumnName("EMAIL");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(100)")
                        .HasColumnName("NOME");

                    b.Property<string>("SenhaHash")
                        .IsRequired()
                        .HasMaxLength(100)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(100)")
                        .HasColumnName("SENHA_HASH");

                    b.Property<string>("Telefone")
                        .HasMaxLength(20)
                        .IsUnicode(false)
                        .HasColumnType("VARCHAR2(20)")
                        .HasColumnName("TELEFONE");

                    b.HasKey("IdTutor");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.ToTable("TUTOR", (string)null);
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.DispositivoIot", b =>
                {
                    b.HasOne("ClyvoElo.Domain.Entities.Pet", "Pet")
                        .WithMany("Dispositivos")
                        .HasForeignKey("IdPet")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Pet");
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Doutor", b =>
                {
                    b.HasOne("ClyvoElo.Domain.Entities.Clinica", "Clinica")
                        .WithMany("Doutores")
                        .HasForeignKey("IdClinica")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Clinica");
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.EventoClinico", b =>
                {
                    b.HasOne("ClyvoElo.Domain.Entities.Doutor", "Doutor")
                        .WithMany()
                        .HasForeignKey("IdDoutor")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("ClyvoElo.Domain.Entities.Pet", "Pet")
                        .WithMany("EventosClinicos")
                        .HasForeignKey("IdPet")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Doutor");

                    b.Navigation("Pet");
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.LeituraSensor", b =>
                {
                    b.HasOne("ClyvoElo.Domain.Entities.DispositivoIot", "Dispositivo")
                        .WithMany("Leituras")
                        .HasForeignKey("IdDispositivo")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Dispositivo");
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Pet", b =>
                {
                    b.HasOne("ClyvoElo.Domain.Entities.Tutor", "Tutor")
                        .WithMany("Pets")
                        .HasForeignKey("IdTutor")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Tutor");
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Clinica", b =>
                {
                    b.Navigation("Doutores");
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.DispositivoIot", b =>
                {
                    b.Navigation("Leituras");
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Pet", b =>
                {
                    b.Navigation("Dispositivos");

                    b.Navigation("EventosClinicos");
                });

            modelBuilder.Entity("ClyvoElo.Domain.Entities.Tutor", b =>
                {
                    b.Navigation("Pets");
                });
#pragma warning restore 612, 618
        }
    }
}

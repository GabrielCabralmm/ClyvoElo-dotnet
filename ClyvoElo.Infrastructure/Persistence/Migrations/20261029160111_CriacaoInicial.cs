using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClyvoElo.Infrastructure.Persistence.Migrations
{
    public partial class CriacaoInicial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLINICA",
                columns: table => new
                {
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: false),
                    TELEFONE = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLINICA", x => x.ID_CLINICA);
                });

            migrationBuilder.CreateTable(
                name: "TUTOR",
                columns: table => new
                {
                    ID_TUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    NOME = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "VARCHAR2(150)", unicode: false, maxLength: 150, nullable: false),
                    SENHA_HASH = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: false),
                    TELEFONE = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: true),
                    DATA_CADASTRO = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TUTOR", x => x.ID_TUTOR);
                });

            migrationBuilder.CreateTable(
                name: "DOUTOR",
                columns: table => new
                {
                    ID_DOUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_CLINICA = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOME = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: false),
                    CRMV = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: false),
                    EMAIL = table.Column<string>(type: "VARCHAR2(150)", unicode: false, maxLength: 150, nullable: false),
                    SENHA_HASH = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DOUTOR", x => x.ID_DOUTOR);
                    table.ForeignKey(
                        name: "FK_DOUTOR_CLINICA_ID_CLINICA",
                        column: x => x.ID_CLINICA,
                        principalTable: "CLINICA",
                        principalColumn: "ID_CLINICA",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PET",
                columns: table => new
                {
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_TUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    NOME = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: false),
                    ESPECIE = table.Column<string>(type: "VARCHAR2(50)", unicode: false, maxLength: 50, nullable: false),
                    RACA = table.Column<string>(type: "VARCHAR2(100)", unicode: false, maxLength: 100, nullable: true),
                    DATA_NASCIMENTO = table.Column<DateTime>(type: "DATE", nullable: true),
                    PESO_KG = table.Column<decimal>(type: "DECIMAL(5,2)", precision: 5, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PET", x => x.ID_PET);
                    table.ForeignKey(
                        name: "FK_PET_TUTOR_ID_TUTOR",
                        column: x => x.ID_TUTOR,
                        principalTable: "TUTOR",
                        principalColumn: "ID_TUTOR",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DISPOSITIVO_IOT",
                columns: table => new
                {
                    ID_DISPOSITIVO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TIPO_DISPOSITIVO = table.Column<string>(type: "VARCHAR2(50)", unicode: false, maxLength: 50, nullable: false),
                    DATA_INSTALACAO = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DISPOSITIVO_IOT", x => x.ID_DISPOSITIVO);
                    table.ForeignKey(
                        name: "FK_DISPOSITIVO_IOT_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EVENTO_CLINICO",
                columns: table => new
                {
                    ID_EVENTO = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_PET = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    ID_DOUTOR = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TIPO_EVENTO = table.Column<string>(type: "VARCHAR2(30)", unicode: false, maxLength: 30, nullable: false),
                    DESCRICAO = table.Column<string>(type: "VARCHAR2(500)", unicode: false, maxLength: 500, nullable: true),
                    DATA_EVENTO = table.Column<DateTime>(type: "DATE", nullable: false),
                    STATUS = table.Column<string>(type: "VARCHAR2(20)", unicode: false, maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EVENTO_CLINICO", x => x.ID_EVENTO);
                    table.CheckConstraint("CK_EVENTO_STATUS", "STATUS IN ('PENDENTE','CONCLUIDO','ATRASADO')");
                    table.CheckConstraint("CK_EVENTO_TIPO", "TIPO_EVENTO IN ('VACINA','CHECKUP','POS_OPERATORIO','MEDICACAO','EXAME')");
                    table.ForeignKey(
                        name: "FK_EVENTO_CLINICO_DOUTOR_ID_DOUTOR",
                        column: x => x.ID_DOUTOR,
                        principalTable: "DOUTOR",
                        principalColumn: "ID_DOUTOR",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EVENTO_CLINICO_PET_ID_PET",
                        column: x => x.ID_PET,
                        principalTable: "PET",
                        principalColumn: "ID_PET",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LEITURA_SENSOR",
                columns: table => new
                {
                    ID_LEITURA = table.Column<int>(type: "NUMBER(10)", nullable: false)
                        .Annotation("Oracle:Identity", "START WITH 1 INCREMENT BY 1"),
                    ID_DISPOSITIVO = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    TIPO_MEDIDA = table.Column<string>(type: "VARCHAR2(50)", unicode: false, maxLength: 50, nullable: false),
                    VALOR_MEDIDO = table.Column<decimal>(type: "DECIMAL(10,2)", precision: 10, scale: 2, nullable: false),
                    DATA_HORA_LEITURA = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEITURA_SENSOR", x => x.ID_LEITURA);
                    table.ForeignKey(
                        name: "FK_LEITURA_SENSOR_DISPOSITIVO_IOT_ID_DISPOSITIVO",
                        column: x => x.ID_DISPOSITIVO,
                        principalTable: "DISPOSITIVO_IOT",
                        principalColumn: "ID_DISPOSITIVO",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DISPOSITIVO_IOT_ID_PET",
                table: "DISPOSITIVO_IOT",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_DOUTOR_CRMV",
                table: "DOUTOR",
                column: "CRMV",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOUTOR_EMAIL",
                table: "DOUTOR",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DOUTOR_ID_CLINICA",
                table: "DOUTOR",
                column: "ID_CLINICA");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_CLINICO_ID_DOUTOR",
                table: "EVENTO_CLINICO",
                column: "ID_DOUTOR");

            migrationBuilder.CreateIndex(
                name: "IX_EVENTO_CLINICO_ID_PET",
                table: "EVENTO_CLINICO",
                column: "ID_PET");

            migrationBuilder.CreateIndex(
                name: "IX_LEITURA_SENSOR_ID_DISPOSITIVO",
                table: "LEITURA_SENSOR",
                column: "ID_DISPOSITIVO");

            migrationBuilder.CreateIndex(
                name: "IX_PET_ID_TUTOR",
                table: "PET",
                column: "ID_TUTOR");

            migrationBuilder.CreateIndex(
                name: "IX_TUTOR_EMAIL",
                table: "TUTOR",
                column: "EMAIL",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EVENTO_CLINICO");

            migrationBuilder.DropTable(
                name: "LEITURA_SENSOR");

            migrationBuilder.DropTable(
                name: "DOUTOR");

            migrationBuilder.DropTable(
                name: "DISPOSITIVO_IOT");

            migrationBuilder.DropTable(
                name: "CLINICA");

            migrationBuilder.DropTable(
                name: "PET");

            migrationBuilder.DropTable(
                name: "TUTOR");
        }
    }
}

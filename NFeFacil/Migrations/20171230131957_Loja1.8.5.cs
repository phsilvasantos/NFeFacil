﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NFeFacil.Migrations
{
    public partial class Loja185 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inutilizacoes",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CNPJ = table.Column<string>(nullable: true),
                    FimRange = table.Column<int>(nullable: false),
                    Homologacao = table.Column<bool>(nullable: false),
                    InicioRange = table.Column<int>(nullable: false),
                    MomentoProcessamento = table.Column<DateTime>(nullable: false),
                    NumeroProtocolo = table.Column<long>(nullable: false),
                    Serie = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inutilizacoes", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inutilizacoes");
        }
    }
}

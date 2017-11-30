﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NFeFacil.Migrations
{
    public partial class Loja16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Comprador",
                table: "Vendas",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "FormaPagamento",
                table: "Vendas",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PrazoEntrega",
                table: "Vendas",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "PrazoPagamento",
                table: "Vendas",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Compradores",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    IdEmpresa = table.Column<Guid>(nullable: false),
                    Nome = table.Column<string>(nullable: true),
                    Telefone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Compradores", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Compradores");

            migrationBuilder.DropColumn(
                name: "Comprador",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "FormaPagamento",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "PrazoEntrega",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "PrazoPagamento",
                table: "Vendas");
        }
    }
}

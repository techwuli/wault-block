using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class ProofFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Fields",
                table: "ClaimDefinitions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofFields",
                table: "ClaimDefinitions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProofFields",
                table: "ClaimDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "Fields",
                table: "ClaimDefinitions",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}

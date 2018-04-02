using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class CredentialSchemas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CredentialSchemas",
                columns: table => new
                {
                    Did = table.Column<string>(nullable: false),
                    Attributes = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Version = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CredentialSchemas", x => x.Did);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CredentialSchemas");
        }
    }
}

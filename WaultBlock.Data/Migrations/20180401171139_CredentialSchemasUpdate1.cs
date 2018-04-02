using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class CredentialSchemasUpdate1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CredentialSchemas",
                table: "CredentialSchemas");

            migrationBuilder.DropColumn(
                name: "Did",
                table: "CredentialSchemas");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CredentialSchemas",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CredentialSchemas",
                table: "CredentialSchemas",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CredentialSchemas",
                table: "CredentialSchemas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CredentialSchemas");

            migrationBuilder.AddColumn<string>(
                name: "Did",
                table: "CredentialSchemas",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CredentialSchemas",
                table: "CredentialSchemas",
                column: "Did");
        }
    }
}

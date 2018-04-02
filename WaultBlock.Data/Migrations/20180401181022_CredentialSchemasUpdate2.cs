using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class CredentialSchemasUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fields",
                table: "ClaimDefinitions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ClaimDefinitions");

            migrationBuilder.DropColumn(
                name: "ProofFields",
                table: "ClaimDefinitions");

            migrationBuilder.DropColumn(
                name: "Published",
                table: "ClaimDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CredentialSchemas",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Attributes",
                table: "CredentialSchemas",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CredentialSchemaId",
                table: "ClaimDefinitions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_CredentialSchemas_UserId",
                table: "CredentialSchemas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimDefinitions_CredentialSchemaId",
                table: "ClaimDefinitions",
                column: "CredentialSchemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimDefinitions_CredentialSchemas_CredentialSchemaId",
                table: "ClaimDefinitions",
                column: "CredentialSchemaId",
                principalTable: "CredentialSchemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CredentialSchemas_AspNetUsers_UserId",
                table: "CredentialSchemas",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaimDefinitions_CredentialSchemas_CredentialSchemaId",
                table: "ClaimDefinitions");

            migrationBuilder.DropForeignKey(
                name: "FK_CredentialSchemas_AspNetUsers_UserId",
                table: "CredentialSchemas");

            migrationBuilder.DropIndex(
                name: "IX_CredentialSchemas_UserId",
                table: "CredentialSchemas");

            migrationBuilder.DropIndex(
                name: "IX_ClaimDefinitions_CredentialSchemaId",
                table: "ClaimDefinitions");

            migrationBuilder.DropColumn(
                name: "CredentialSchemaId",
                table: "ClaimDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "CredentialSchemas",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Attributes",
                table: "CredentialSchemas",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "Fields",
                table: "ClaimDefinitions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ClaimDefinitions",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProofFields",
                table: "ClaimDefinitions",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Published",
                table: "ClaimDefinitions",
                nullable: false,
                defaultValue: false);
        }
    }
}

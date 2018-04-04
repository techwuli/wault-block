using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaultBlock.Data.Migrations
{
    public partial class CredentialSchemasUpdate3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaimDefinitions_AspNetUsers_UserId",
                table: "ClaimDefinitions");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ClaimDefinitions",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "UserIndyClaims",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClaimDefinitionId = table.Column<Guid>(nullable: false),
                    Issued = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserIndyClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserIndyClaims_ClaimDefinitions_ClaimDefinitionId",
                        column: x => x.ClaimDefinitionId,
                        principalTable: "ClaimDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserIndyClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserIndyClaims_ClaimDefinitionId",
                table: "UserIndyClaims",
                column: "ClaimDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserIndyClaims_UserId",
                table: "UserIndyClaims",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimDefinitions_AspNetUsers_UserId",
                table: "ClaimDefinitions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClaimDefinitions_AspNetUsers_UserId",
                table: "ClaimDefinitions");

            migrationBuilder.DropTable(
                name: "UserIndyClaims");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ClaimDefinitions",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ClaimDefinitions_AspNetUsers_UserId",
                table: "ClaimDefinitions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

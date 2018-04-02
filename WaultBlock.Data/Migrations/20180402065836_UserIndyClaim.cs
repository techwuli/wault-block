using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class UserIndyClaim : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Issued",
                table: "UserIndyClaims");

            migrationBuilder.AddColumn<string>(
                name: "ClaimRequest",
                table: "UserIndyClaims",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClaimResponse",
                table: "UserIndyClaims",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "UserIndyClaims",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClaimRequest",
                table: "UserIndyClaims");

            migrationBuilder.DropColumn(
                name: "ClaimResponse",
                table: "UserIndyClaims");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "UserIndyClaims");

            migrationBuilder.AddColumn<bool>(
                name: "Issued",
                table: "UserIndyClaims",
                nullable: false,
                defaultValue: false);
        }
    }
}

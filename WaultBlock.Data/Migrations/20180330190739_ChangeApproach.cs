using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class ChangeApproach : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Did",
                table: "WaultWallets",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOpened",
                table: "WaultWallets",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Verkey",
                table: "WaultWallets",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Did",
                table: "WaultWallets");

            migrationBuilder.DropColumn(
                name: "LastOpened",
                table: "WaultWallets");

            migrationBuilder.DropColumn(
                name: "Verkey",
                table: "WaultWallets");
        }
    }
}

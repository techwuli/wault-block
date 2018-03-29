using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class UpdateWallets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaultWalletRecords_WaultWallets_WaultWalletId",
                table: "WaultWalletRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaultWallets",
                table: "WaultWallets");

            migrationBuilder.DropIndex(
                name: "IX_WaultWalletRecords_WaultWalletId",
                table: "WaultWalletRecords");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WaultWallets");

            migrationBuilder.DropColumn(
                name: "WaultWalletId",
                table: "WaultWalletRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WaultWallets",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "WaultWalletRecords",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WaultWalletName",
                table: "WaultWalletRecords",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaultWallets",
                table: "WaultWallets",
                columns: new[] { "Name", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_WaultWalletRecords_WaultWalletName_UserId",
                table: "WaultWalletRecords",
                columns: new[] { "WaultWalletName", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_WaultWalletRecords_WaultWallets_WaultWalletName_UserId",
                table: "WaultWalletRecords",
                columns: new[] { "WaultWalletName", "UserId" },
                principalTable: "WaultWallets",
                principalColumns: new[] { "Name", "UserId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaultWalletRecords_WaultWallets_WaultWalletName_UserId",
                table: "WaultWalletRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaultWallets",
                table: "WaultWallets");

            migrationBuilder.DropIndex(
                name: "IX_WaultWalletRecords_WaultWalletName_UserId",
                table: "WaultWalletRecords");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "WaultWalletRecords");

            migrationBuilder.DropColumn(
                name: "WaultWalletName",
                table: "WaultWalletRecords");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "WaultWallets",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "WaultWallets",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "WaultWalletId",
                table: "WaultWalletRecords",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaultWallets",
                table: "WaultWallets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WaultWalletRecords_WaultWalletId",
                table: "WaultWalletRecords",
                column: "WaultWalletId");

            migrationBuilder.AddForeignKey(
                name: "FK_WaultWalletRecords_WaultWallets_WaultWalletId",
                table: "WaultWalletRecords",
                column: "WaultWalletId",
                principalTable: "WaultWallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class CorrectWalletRecordsName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WaultRecords_WalletDatas_WalletName_UserId",
                table: "WaultRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WaultRecords",
                table: "WaultRecords");

            migrationBuilder.RenameTable(
                name: "WaultRecords",
                newName: "WalletRecords");

            migrationBuilder.RenameIndex(
                name: "IX_WaultRecords_WalletName_UserId",
                table: "WalletRecords",
                newName: "IX_WalletRecords_WalletName_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletRecords",
                table: "WalletRecords",
                column: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletRecords_WalletDatas_WalletName_UserId",
                table: "WalletRecords",
                columns: new[] { "WalletName", "UserId" },
                principalTable: "WalletDatas",
                principalColumns: new[] { "Name", "UserId" },
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletRecords_WalletDatas_WalletName_UserId",
                table: "WalletRecords");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletRecords",
                table: "WalletRecords");

            migrationBuilder.RenameTable(
                name: "WalletRecords",
                newName: "WaultRecords");

            migrationBuilder.RenameIndex(
                name: "IX_WalletRecords_WalletName_UserId",
                table: "WaultRecords",
                newName: "IX_WaultRecords_WalletName_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WaultRecords",
                table: "WaultRecords",
                column: "Key");

            migrationBuilder.AddForeignKey(
                name: "FK_WaultRecords_WalletDatas_WalletName_UserId",
                table: "WaultRecords",
                columns: new[] { "WalletName", "UserId" },
                principalTable: "WalletDatas",
                principalColumns: new[] { "Name", "UserId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}

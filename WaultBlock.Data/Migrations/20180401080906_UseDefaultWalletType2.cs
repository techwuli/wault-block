using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class UseDefaultWalletType2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WalletDatas_Id",
                table: "WalletDatas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WalletDatas");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "WalletDatas",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WalletDatas_Id",
                table: "WalletDatas",
                column: "Id");
        }
    }
}

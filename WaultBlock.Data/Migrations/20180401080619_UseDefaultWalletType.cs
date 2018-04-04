using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class UseDefaultWalletType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WalletRecords");

            migrationBuilder.DropColumn(
                name: "FreshnessDuration",
                table: "WalletDatas");

            migrationBuilder.DropColumn(
                name: "IsOpen",
                table: "WalletDatas");

            migrationBuilder.DropColumn(
                name: "LastOpened",
                table: "WalletDatas");

            migrationBuilder.AddColumn<string>(
                name: "Did",
                table: "WalletDatas",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "WalletDatas",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeCreated",
                table: "WalletDatas",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "VerKey",
                table: "WalletDatas",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_WalletDatas_Id",
                table: "WalletDatas",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_WalletDatas_Id",
                table: "WalletDatas");

            migrationBuilder.DropColumn(
                name: "Did",
                table: "WalletDatas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "WalletDatas");

            migrationBuilder.DropColumn(
                name: "TimeCreated",
                table: "WalletDatas");

            migrationBuilder.DropColumn(
                name: "VerKey",
                table: "WalletDatas");

            migrationBuilder.AddColumn<double>(
                name: "FreshnessDuration",
                table: "WalletDatas",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsOpen",
                table: "WalletDatas",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOpened",
                table: "WalletDatas",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WalletRecords",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    WalletName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletRecords", x => x.Key);
                    table.ForeignKey(
                        name: "FK_WalletRecords_WalletDatas_WalletName_UserId",
                        columns: x => new { x.WalletName, x.UserId },
                        principalTable: "WalletDatas",
                        principalColumns: new[] { "Name", "UserId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletRecords_WalletName_UserId",
                table: "WalletRecords",
                columns: new[] { "WalletName", "UserId" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WaultBlock.Data.Migrations
{
    public partial class RefactorFirstTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaultWalletRecords");

            migrationBuilder.DropTable(
                name: "WaultWallets");

            migrationBuilder.CreateTable(
                name: "WalletDatas",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    FreshnessDuration = table.Column<double>(nullable: true),
                    IsOpen = table.Column<bool>(nullable: false),
                    LastOpened = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletDatas", x => new { x.Name, x.UserId });
                    table.ForeignKey(
                        name: "FK_WalletDatas_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaultRecords",
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
                    table.PrimaryKey("PK_WaultRecords", x => x.Key);
                    table.ForeignKey(
                        name: "FK_WaultRecords_WalletDatas_WalletName_UserId",
                        columns: x => new { x.WalletName, x.UserId },
                        principalTable: "WalletDatas",
                        principalColumns: new[] { "Name", "UserId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WalletDatas_UserId",
                table: "WalletDatas",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WaultRecords_WalletName_UserId",
                table: "WaultRecords",
                columns: new[] { "WalletName", "UserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WaultRecords");

            migrationBuilder.DropTable(
                name: "WalletDatas");

            migrationBuilder.CreateTable(
                name: "WaultWallets",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Did = table.Column<string>(nullable: true),
                    FreshnessDuration = table.Column<double>(nullable: true),
                    IsOpen = table.Column<bool>(nullable: false),
                    LastOpened = table.Column<DateTime>(nullable: true),
                    Verkey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaultWallets", x => new { x.Name, x.UserId });
                    table.ForeignKey(
                        name: "FK_WaultWallets_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WaultWalletRecords",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    TimeCreated = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    WaultWalletName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaultWalletRecords", x => x.Key);
                    table.ForeignKey(
                        name: "FK_WaultWalletRecords_WaultWallets_WaultWalletName_UserId",
                        columns: x => new { x.WaultWalletName, x.UserId },
                        principalTable: "WaultWallets",
                        principalColumns: new[] { "Name", "UserId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WaultWalletRecords_WaultWalletName_UserId",
                table: "WaultWalletRecords",
                columns: new[] { "WaultWalletName", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_WaultWallets_UserId",
                table: "WaultWallets",
                column: "UserId");
        }
    }
}

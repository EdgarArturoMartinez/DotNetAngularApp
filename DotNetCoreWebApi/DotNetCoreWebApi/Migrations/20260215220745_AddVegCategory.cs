using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVegCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "VegProducts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCategory",
                table: "VegProducts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VegCategories",
                columns: table => new
                {
                    IdCategory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VegCategories", x => x.IdCategory);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VegProducts_IdCategory",
                table: "VegProducts",
                column: "IdCategory");

            migrationBuilder.AddForeignKey(
                name: "FK_VegProducts_VegCategories_IdCategory",
                table: "VegProducts",
                column: "IdCategory",
                principalTable: "VegCategories",
                principalColumn: "IdCategory",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VegProducts_VegCategories_IdCategory",
                table: "VegProducts");

            migrationBuilder.DropTable(
                name: "VegCategories");

            migrationBuilder.DropIndex(
                name: "IX_VegProducts_IdCategory",
                table: "VegProducts");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "VegProducts");

            migrationBuilder.DropColumn(
                name: "IdCategory",
                table: "VegProducts");
        }
    }
}

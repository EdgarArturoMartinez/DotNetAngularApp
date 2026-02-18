using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreWebApi.Migrations
{
    /// <inheritdoc />
    public partial class StockQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
            name: "StockQuantity",
            table: "VegProducts",
            type: "int",
            nullable: false,
            defaultValue: 0);  // Default value
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
            name: "StockQuantity",
            table: "VegProducts");
        }
    }
}

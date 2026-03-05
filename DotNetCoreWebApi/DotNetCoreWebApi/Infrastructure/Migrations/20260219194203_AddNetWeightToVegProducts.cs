using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreWebApi.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNetWeightToVegProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NetWeight",
                table: "VegProducts",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NetWeight",
                table: "VegProducts");
        }
    }
}

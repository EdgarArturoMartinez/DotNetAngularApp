using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotNetCoreWebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddVegTypeWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create VegTypeWeight table
            migrationBuilder.CreateTable(
                name: "VegTypeWeights",
                columns: table => new
                {
                    IdTypeWeight = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AbbreviationWeight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VegTypeWeights", x => x.IdTypeWeight);
                });

            // Insert seed data
            migrationBuilder.InsertData(
                table: "VegTypeWeights",
                columns: new[] { "IdTypeWeight", "Name", "AbbreviationWeight", "Description", "IsActive", "CreatedAt" },
                values: new object[,]
                {
                    { 1, "Grames", "Gms", "Grams weight measurement", true, DateTime.Now },
                    { 2, "Onzas", "Oz", "Ounces weight measurement", true, DateTime.Now },
                    { 3, "Liters", "Lts", "Liters volume measurement", true, DateTime.Now },
                    { 4, "Kilograms", "Kg", "Kilograms weight measurement", true, DateTime.Now },
                    { 5, "Pounds", "Lb", "Pounds weight measurement", true, DateTime.Now },
                    { 6, "Milliliters", "ml", "Milliliters volume measurement", true, DateTime.Now }
                });

            // Add IdTypeWeight column to VegProducts
            migrationBuilder.AddColumn<int>(
                name: "IdTypeWeight",
                table: "VegProducts",
                type: "int",
                nullable: true);

            // Create foreign key constraint
            migrationBuilder.CreateIndex(
                name: "IX_VegProducts_IdTypeWeight",
                table: "VegProducts",
                column: "IdTypeWeight");

            migrationBuilder.AddForeignKey(
                name: "FK_VegProducts_VegTypeWeights_IdTypeWeight",
                table: "VegProducts",
                column: "IdTypeWeight",
                principalTable: "VegTypeWeights",
                principalColumn: "IdTypeWeight",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key
            migrationBuilder.DropForeignKey(
                name: "FK_VegProducts_VegTypeWeights_IdTypeWeight",
                table: "VegProducts");

            // Drop index
            migrationBuilder.DropIndex(
                name: "IX_VegProducts_IdTypeWeight",
                table: "VegProducts");

            // Drop IdTypeWeight column from VegProducts
            migrationBuilder.DropColumn(
                name: "IdTypeWeight",
                table: "VegProducts");

            // Drop VegTypeWeights table
            migrationBuilder.DropTable(
                name: "VegTypeWeights");
        }
    }
}

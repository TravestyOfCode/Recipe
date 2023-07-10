using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Link_Ingredient_with_Product : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Product",
                table: "Ingredient");

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Ingredient",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredient_ProductId",
                table: "Ingredient",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredient_Product_ProductId",
                table: "Ingredient",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredient_Product_ProductId",
                table: "Ingredient");

            migrationBuilder.DropIndex(
                name: "IX_Ingredient_ProductId",
                table: "Ingredient");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Ingredient");

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "Ingredient",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");
        }
    }
}

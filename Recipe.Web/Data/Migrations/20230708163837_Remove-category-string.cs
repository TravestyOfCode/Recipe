using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Recipe.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Removecategorystring : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryRecipe_Category_CategoryListId",
                table: "CategoryRecipe");

            migrationBuilder.DropColumn(
                name: "Categories",
                table: "Recipes");

            migrationBuilder.RenameColumn(
                name: "CategoryListId",
                table: "CategoryRecipe",
                newName: "CategoriesId");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryRecipe_Category_CategoriesId",
                table: "CategoryRecipe",
                column: "CategoriesId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CategoryRecipe_Category_CategoriesId",
                table: "CategoryRecipe");

            migrationBuilder.RenameColumn(
                name: "CategoriesId",
                table: "CategoryRecipe",
                newName: "CategoryListId");

            migrationBuilder.AddColumn<string>(
                name: "Categories",
                table: "Recipes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryRecipe_Category_CategoryListId",
                table: "CategoryRecipe",
                column: "CategoryListId",
                principalTable: "Category",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

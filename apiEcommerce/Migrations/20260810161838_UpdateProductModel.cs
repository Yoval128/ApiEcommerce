using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace apiEcommerce.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "imgUrlLocal",
                table: "Products",
                newName: "ImgUrlLocal");

            migrationBuilder.RenameColumn(
                name: "imgUrl",
                table: "Products",
                newName: "ImgUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImgUrlLocal",
                table: "Products",
                newName: "imgUrlLocal");

            migrationBuilder.RenameColumn(
                name: "ImgUrl",
                table: "Products",
                newName: "imgUrl");
        }
    }
}

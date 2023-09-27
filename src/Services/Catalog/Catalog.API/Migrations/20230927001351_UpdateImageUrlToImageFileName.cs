using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.API.Migrations
{
    public partial class UpdateImageUrlToImageFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThumbnailUrl",
                table: "Products",
                newName: "ThumbnailFileName");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductImages",
                newName: "ImageFileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ThumbnailFileName",
                table: "Products",
                newName: "ThumbnailUrl");

            migrationBuilder.RenameColumn(
                name: "ImageFileName",
                table: "ProductImages",
                newName: "ImageUrl");
        }
    }
}

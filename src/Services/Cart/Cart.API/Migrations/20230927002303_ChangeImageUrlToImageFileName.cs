using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cart.API.Migrations
{
    public partial class ChangeImageUrlToImageFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductThumbnailUrl",
                table: "CartItems",
                newName: "ProductThumbnailFileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductThumbnailFileName",
                table: "CartItems",
                newName: "ProductThumbnailUrl");
        }
    }
}

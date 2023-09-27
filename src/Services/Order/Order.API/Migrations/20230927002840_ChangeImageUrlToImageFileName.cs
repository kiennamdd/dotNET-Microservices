using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.API.Migrations
{
    public partial class ChangeImageUrlToImageFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductThumbnailUrl",
                table: "OrderItems",
                newName: "ProductThumbnailFileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductThumbnailFileName",
                table: "OrderItems",
                newName: "ProductThumbnailUrl");
        }
    }
}

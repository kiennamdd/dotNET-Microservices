using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cart.API.Migrations
{
    public partial class AddColumnToCartItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductThumbnailUrl",
                table: "CartItems",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductThumbnailUrl",
                table: "CartItems");
        }
    }
}

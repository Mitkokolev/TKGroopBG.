using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKGroopBG.Migrations
{
    /// <inheritdoc />
    public partial class CartAndOrderFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Details",
                table: "Cart",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Details",
                table: "Cart");
        }
    }
}

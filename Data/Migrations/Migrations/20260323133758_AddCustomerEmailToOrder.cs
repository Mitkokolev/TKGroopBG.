using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TKGroopBG.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerEmailToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Orders");
        }
    }
}

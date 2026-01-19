using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBillsAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderToBlockElementId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "ContentBlocks");

            migrationBuilder.AddColumn<string>(
                name: "Slot",
                table: "ContentBlocks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slot",
                table: "ContentBlocks");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ContentBlocks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

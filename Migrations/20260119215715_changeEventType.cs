using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBillsAPI.Migrations
{
    /// <inheritdoc />
    public partial class changeEventType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "EventTypes",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "EventTypes",
                newName: "Code");
        }
    }
}

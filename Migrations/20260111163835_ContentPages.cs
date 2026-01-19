using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBillsAPI.Migrations
{
    /// <inheritdoc />
    public partial class ContentPages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentBlock_ContentPage_ContentPageId",
                table: "ContentBlock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentBlock",
                table: "ContentBlock");

            migrationBuilder.RenameTable(
                name: "ContentBlock",
                newName: "ContentBlocks");

            migrationBuilder.RenameIndex(
                name: "IX_ContentBlock_ContentPageId",
                table: "ContentBlocks",
                newName: "IX_ContentBlocks_ContentPageId");

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ContentBlocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentBlocks",
                table: "ContentBlocks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentBlocks_ContentPage_ContentPageId",
                table: "ContentBlocks",
                column: "ContentPageId",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContentBlocks_ContentPage_ContentPageId",
                table: "ContentBlocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContentBlocks",
                table: "ContentBlocks");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ContentBlocks");

            migrationBuilder.RenameTable(
                name: "ContentBlocks",
                newName: "ContentBlock");

            migrationBuilder.RenameIndex(
                name: "IX_ContentBlocks_ContentPageId",
                table: "ContentBlock",
                newName: "IX_ContentBlock_ContentPageId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContentBlock",
                table: "ContentBlock",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ContentBlock_ContentPage_ContentPageId",
                table: "ContentBlock",
                column: "ContentPageId",
                principalTable: "ContentPage",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

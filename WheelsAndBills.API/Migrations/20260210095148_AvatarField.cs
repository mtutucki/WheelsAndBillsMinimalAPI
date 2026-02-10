using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBills.API.Migrations
{
    /// <inheritdoc />
    public partial class AvatarField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AvatarFileId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvatarFileId",
                table: "AspNetUsers");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBills.API.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAspNetUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExperienceLevelId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExperienceLevelId",
                table: "AspNetUsers");
        }
    }
}

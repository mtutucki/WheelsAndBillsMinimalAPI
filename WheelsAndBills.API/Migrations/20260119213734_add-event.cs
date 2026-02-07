using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBills.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cost_VehicleEvent_VehicleEventId",
                table: "Cost");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairEvent_VehicleEvent_VehicleEventId",
                table: "RepairEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleEvent_EventType_EventTypeId",
                table: "VehicleEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleEvent_Vehicles_VehicleId",
                table: "VehicleEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleNote_AspNetUsers_UserId",
                table: "VehicleNote");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleNote_Vehicles_VehicleId",
                table: "VehicleNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleNote",
                table: "VehicleNote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleEvent",
                table: "VehicleEvent");

            migrationBuilder.RenameTable(
                name: "VehicleNote",
                newName: "VehicleNotes");

            migrationBuilder.RenameTable(
                name: "VehicleEvent",
                newName: "VehicleEvents");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleNote_VehicleId",
                table: "VehicleNotes",
                newName: "IX_VehicleNotes_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleNote_UserId",
                table: "VehicleNotes",
                newName: "IX_VehicleNotes_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleEvent_VehicleId",
                table: "VehicleEvents",
                newName: "IX_VehicleEvents_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleEvent_EventTypeId",
                table: "VehicleEvents",
                newName: "IX_VehicleEvents_EventTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleNotes",
                table: "VehicleNotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleEvents",
                table: "VehicleEvents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cost_VehicleEvents_VehicleEventId",
                table: "Cost",
                column: "VehicleEventId",
                principalTable: "VehicleEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairEvent_VehicleEvents_VehicleEventId",
                table: "RepairEvent",
                column: "VehicleEventId",
                principalTable: "VehicleEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleEvents_EventType_EventTypeId",
                table: "VehicleEvents",
                column: "EventTypeId",
                principalTable: "EventType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleEvents_Vehicles_VehicleId",
                table: "VehicleEvents",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleNotes_AspNetUsers_UserId",
                table: "VehicleNotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleNotes_Vehicles_VehicleId",
                table: "VehicleNotes",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cost_VehicleEvents_VehicleEventId",
                table: "Cost");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairEvent_VehicleEvents_VehicleEventId",
                table: "RepairEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleEvents_EventType_EventTypeId",
                table: "VehicleEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleEvents_Vehicles_VehicleId",
                table: "VehicleEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleNotes_AspNetUsers_UserId",
                table: "VehicleNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleNotes_Vehicles_VehicleId",
                table: "VehicleNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleNotes",
                table: "VehicleNotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleEvents",
                table: "VehicleEvents");

            migrationBuilder.RenameTable(
                name: "VehicleNotes",
                newName: "VehicleNote");

            migrationBuilder.RenameTable(
                name: "VehicleEvents",
                newName: "VehicleEvent");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleNotes_VehicleId",
                table: "VehicleNote",
                newName: "IX_VehicleNote_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleNotes_UserId",
                table: "VehicleNote",
                newName: "IX_VehicleNote_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleEvents_VehicleId",
                table: "VehicleEvent",
                newName: "IX_VehicleEvent_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleEvents_EventTypeId",
                table: "VehicleEvent",
                newName: "IX_VehicleEvent_EventTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleNote",
                table: "VehicleNote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleEvent",
                table: "VehicleEvent",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cost_VehicleEvent_VehicleEventId",
                table: "Cost",
                column: "VehicleEventId",
                principalTable: "VehicleEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairEvent_VehicleEvent_VehicleEventId",
                table: "RepairEvent",
                column: "VehicleEventId",
                principalTable: "VehicleEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleEvent_EventType_EventTypeId",
                table: "VehicleEvent",
                column: "EventTypeId",
                principalTable: "EventType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleEvent_Vehicles_VehicleId",
                table: "VehicleEvent",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleNote_AspNetUsers_UserId",
                table: "VehicleNote",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleNote_Vehicles_VehicleId",
                table: "VehicleNote",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WheelsAndBillsAPI.Migrations
{
    /// <inheritdoc />
    public partial class addallTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cost_CostType_CostTypeId",
                table: "Cost");

            migrationBuilder.DropForeignKey(
                name: "FK_Cost_VehicleEvents_VehicleEventId",
                table: "Cost");

            migrationBuilder.DropForeignKey(
                name: "FK_EventPart_Part_PartId",
                table: "EventPart");

            migrationBuilder.DropForeignKey(
                name: "FK_EventPart_RepairEvent_RepairEventId",
                table: "EventPart");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_AspNetUsers_UserId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Vehicles_VehicleId",
                table: "Notification");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairEvent_VehicleEvents_VehicleEventId",
                table: "RepairEvent");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportDefinition_ReportDefinitionId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleEvents_EventType_EventTypeId",
                table: "VehicleEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModel_VehicleBrand_BrandId",
                table: "VehicleModel");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleBrand_BrandId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModel_ModelId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleStatus_StatusId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleType_TypeId",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleType",
                table: "VehicleType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleStatus",
                table: "VehicleStatus");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleModel",
                table: "VehicleModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleBrand",
                table: "VehicleBrand");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportDefinition",
                table: "ReportDefinition");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepairEvent",
                table: "RepairEvent");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Part",
                table: "Part");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notification",
                table: "Notification");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventType",
                table: "EventType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventPart",
                table: "EventPart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostType",
                table: "CostType");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Cost",
                table: "Cost");

            migrationBuilder.RenameTable(
                name: "VehicleType",
                newName: "VehicleTypes");

            migrationBuilder.RenameTable(
                name: "VehicleStatus",
                newName: "VehicleStatuses");

            migrationBuilder.RenameTable(
                name: "VehicleModel",
                newName: "VehicleModels");

            migrationBuilder.RenameTable(
                name: "VehicleBrand",
                newName: "VehicleBrands");

            migrationBuilder.RenameTable(
                name: "ReportDefinition",
                newName: "ReportDefinitions");

            migrationBuilder.RenameTable(
                name: "RepairEvent",
                newName: "RepairEvents");

            migrationBuilder.RenameTable(
                name: "Part",
                newName: "Parts");

            migrationBuilder.RenameTable(
                name: "Notification",
                newName: "Notifications");

            migrationBuilder.RenameTable(
                name: "EventType",
                newName: "EventTypes");

            migrationBuilder.RenameTable(
                name: "EventPart",
                newName: "EventParts");

            migrationBuilder.RenameTable(
                name: "CostType",
                newName: "CostTypes");

            migrationBuilder.RenameTable(
                name: "Cost",
                newName: "Costs");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleModel_BrandId",
                table: "VehicleModels",
                newName: "IX_VehicleModels_BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_RepairEvent_VehicleEventId",
                table: "RepairEvents",
                newName: "IX_RepairEvents_VehicleEventId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_VehicleId",
                table: "Notifications",
                newName: "IX_Notifications_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_Notification_UserId",
                table: "Notifications",
                newName: "IX_Notifications_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventPart_PartId",
                table: "EventParts",
                newName: "IX_EventParts_PartId");

            migrationBuilder.RenameIndex(
                name: "IX_Cost_VehicleEventId",
                table: "Costs",
                newName: "IX_Costs_VehicleEventId");

            migrationBuilder.RenameIndex(
                name: "IX_Cost_CostTypeId",
                table: "Costs",
                newName: "IX_Costs_CostTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleTypes",
                table: "VehicleTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleStatuses",
                table: "VehicleStatuses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleModels",
                table: "VehicleModels",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleBrands",
                table: "VehicleBrands",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportDefinitions",
                table: "ReportDefinitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepairEvents",
                table: "RepairEvents",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Parts",
                table: "Parts",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventTypes",
                table: "EventTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventParts",
                table: "EventParts",
                columns: new[] { "RepairEventId", "PartId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostTypes",
                table: "CostTypes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Costs",
                table: "Costs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Dictionaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dictionaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DictionaryItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DictionaryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileResources",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileResources", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FuelingEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Liters = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuelingEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FuelingEvents_VehicleEvents_VehicleEventId",
                        column: x => x.VehicleEventId,
                        principalTable: "VehicleEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedReports_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RecurringCosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CostTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IntervalMonths = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecurringCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecurringCosts_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportParameters",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportParameters_Reports_ReportId",
                        column: x => x.ReportId,
                        principalTable: "Reports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Workshops",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workshops", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VehicleEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkshopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceEvents_VehicleEvents_VehicleEventId",
                        column: x => x.VehicleEventId,
                        principalTable: "VehicleEvents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceEvents_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuelingEvents_VehicleEventId",
                table: "FuelingEvents",
                column: "VehicleEventId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedReports_ReportId",
                table: "GeneratedReports",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_RecurringCosts_VehicleId",
                table: "RecurringCosts",
                column: "VehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportParameters_ReportId",
                table: "ReportParameters",
                column: "ReportId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEvents_VehicleEventId",
                table: "ServiceEvents",
                column: "VehicleEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceEvents_WorkshopId",
                table: "ServiceEvents",
                column: "WorkshopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Costs_CostTypes_CostTypeId",
                table: "Costs",
                column: "CostTypeId",
                principalTable: "CostTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Costs_VehicleEvents_VehicleEventId",
                table: "Costs",
                column: "VehicleEventId",
                principalTable: "VehicleEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventParts_Parts_PartId",
                table: "EventParts",
                column: "PartId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventParts_RepairEvents_RepairEventId",
                table: "EventParts",
                column: "RepairEventId",
                principalTable: "RepairEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Vehicles_VehicleId",
                table: "Notifications",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairEvents_VehicleEvents_VehicleEventId",
                table: "RepairEvents",
                column: "VehicleEventId",
                principalTable: "VehicleEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportDefinitions_ReportDefinitionId",
                table: "Reports",
                column: "ReportDefinitionId",
                principalTable: "ReportDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleEvents_EventTypes_EventTypeId",
                table: "VehicleEvents",
                column: "EventTypeId",
                principalTable: "EventTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_VehicleBrands_BrandId",
                table: "VehicleModels",
                column: "BrandId",
                principalTable: "VehicleBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleBrands_BrandId",
                table: "Vehicles",
                column: "BrandId",
                principalTable: "VehicleBrands",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles",
                column: "ModelId",
                principalTable: "VehicleModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleStatuses_StatusId",
                table: "Vehicles",
                column: "StatusId",
                principalTable: "VehicleStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleTypes_TypeId",
                table: "Vehicles",
                column: "TypeId",
                principalTable: "VehicleTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Costs_CostTypes_CostTypeId",
                table: "Costs");

            migrationBuilder.DropForeignKey(
                name: "FK_Costs_VehicleEvents_VehicleEventId",
                table: "Costs");

            migrationBuilder.DropForeignKey(
                name: "FK_EventParts_Parts_PartId",
                table: "EventParts");

            migrationBuilder.DropForeignKey(
                name: "FK_EventParts_RepairEvents_RepairEventId",
                table: "EventParts");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_AspNetUsers_UserId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Vehicles_VehicleId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_RepairEvents_VehicleEvents_VehicleEventId",
                table: "RepairEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_Reports_ReportDefinitions_ReportDefinitionId",
                table: "Reports");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleEvents_EventTypes_EventTypeId",
                table: "VehicleEvents");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleBrands_BrandId",
                table: "VehicleModels");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleBrands_BrandId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleModels_ModelId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleStatuses_StatusId",
                table: "Vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicles_VehicleTypes_TypeId",
                table: "Vehicles");

            migrationBuilder.DropTable(
                name: "Dictionaries");

            migrationBuilder.DropTable(
                name: "DictionaryItems");

            migrationBuilder.DropTable(
                name: "FileResources");

            migrationBuilder.DropTable(
                name: "FuelingEvents");

            migrationBuilder.DropTable(
                name: "GeneratedReports");

            migrationBuilder.DropTable(
                name: "NotificationTypes");

            migrationBuilder.DropTable(
                name: "RecurringCosts");

            migrationBuilder.DropTable(
                name: "ReportParameters");

            migrationBuilder.DropTable(
                name: "ServiceEvents");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Workshops");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleTypes",
                table: "VehicleTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleStatuses",
                table: "VehicleStatuses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleModels",
                table: "VehicleModels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VehicleBrands",
                table: "VehicleBrands");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ReportDefinitions",
                table: "ReportDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RepairEvents",
                table: "RepairEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Parts",
                table: "Parts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notifications",
                table: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventTypes",
                table: "EventTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventParts",
                table: "EventParts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CostTypes",
                table: "CostTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Costs",
                table: "Costs");

            migrationBuilder.RenameTable(
                name: "VehicleTypes",
                newName: "VehicleType");

            migrationBuilder.RenameTable(
                name: "VehicleStatuses",
                newName: "VehicleStatus");

            migrationBuilder.RenameTable(
                name: "VehicleModels",
                newName: "VehicleModel");

            migrationBuilder.RenameTable(
                name: "VehicleBrands",
                newName: "VehicleBrand");

            migrationBuilder.RenameTable(
                name: "ReportDefinitions",
                newName: "ReportDefinition");

            migrationBuilder.RenameTable(
                name: "RepairEvents",
                newName: "RepairEvent");

            migrationBuilder.RenameTable(
                name: "Parts",
                newName: "Part");

            migrationBuilder.RenameTable(
                name: "Notifications",
                newName: "Notification");

            migrationBuilder.RenameTable(
                name: "EventTypes",
                newName: "EventType");

            migrationBuilder.RenameTable(
                name: "EventParts",
                newName: "EventPart");

            migrationBuilder.RenameTable(
                name: "CostTypes",
                newName: "CostType");

            migrationBuilder.RenameTable(
                name: "Costs",
                newName: "Cost");

            migrationBuilder.RenameIndex(
                name: "IX_VehicleModels_BrandId",
                table: "VehicleModel",
                newName: "IX_VehicleModel_BrandId");

            migrationBuilder.RenameIndex(
                name: "IX_RepairEvents_VehicleEventId",
                table: "RepairEvent",
                newName: "IX_RepairEvent_VehicleEventId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_VehicleId",
                table: "Notification",
                newName: "IX_Notification_VehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_UserId",
                table: "Notification",
                newName: "IX_Notification_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_EventParts_PartId",
                table: "EventPart",
                newName: "IX_EventPart_PartId");

            migrationBuilder.RenameIndex(
                name: "IX_Costs_VehicleEventId",
                table: "Cost",
                newName: "IX_Cost_VehicleEventId");

            migrationBuilder.RenameIndex(
                name: "IX_Costs_CostTypeId",
                table: "Cost",
                newName: "IX_Cost_CostTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleType",
                table: "VehicleType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleStatus",
                table: "VehicleStatus",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleModel",
                table: "VehicleModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VehicleBrand",
                table: "VehicleBrand",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ReportDefinition",
                table: "ReportDefinition",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RepairEvent",
                table: "RepairEvent",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Part",
                table: "Part",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notification",
                table: "Notification",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventType",
                table: "EventType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventPart",
                table: "EventPart",
                columns: new[] { "RepairEventId", "PartId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CostType",
                table: "CostType",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cost",
                table: "Cost",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cost_CostType_CostTypeId",
                table: "Cost",
                column: "CostTypeId",
                principalTable: "CostType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cost_VehicleEvents_VehicleEventId",
                table: "Cost",
                column: "VehicleEventId",
                principalTable: "VehicleEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventPart_Part_PartId",
                table: "EventPart",
                column: "PartId",
                principalTable: "Part",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventPart_RepairEvent_RepairEventId",
                table: "EventPart",
                column: "RepairEventId",
                principalTable: "RepairEvent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_AspNetUsers_UserId",
                table: "Notification",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Vehicles_VehicleId",
                table: "Notification",
                column: "VehicleId",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RepairEvent_VehicleEvents_VehicleEventId",
                table: "RepairEvent",
                column: "VehicleEventId",
                principalTable: "VehicleEvents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_ReportDefinition_ReportDefinitionId",
                table: "Reports",
                column: "ReportDefinitionId",
                principalTable: "ReportDefinition",
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
                name: "FK_VehicleModel_VehicleBrand_BrandId",
                table: "VehicleModel",
                column: "BrandId",
                principalTable: "VehicleBrand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleBrand_BrandId",
                table: "Vehicles",
                column: "BrandId",
                principalTable: "VehicleBrand",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleModel_ModelId",
                table: "Vehicles",
                column: "ModelId",
                principalTable: "VehicleModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleStatus_StatusId",
                table: "Vehicles",
                column: "StatusId",
                principalTable: "VehicleStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicles_VehicleType_TypeId",
                table: "Vehicles",
                column: "TypeId",
                principalTable: "VehicleType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

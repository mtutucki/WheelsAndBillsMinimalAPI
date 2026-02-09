using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using WheelsAndBills.Infrastructure.Persistence;

#nullable disable

namespace WheelsAndBills.API.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260209193000_AddNotificationIsRead")]
    /// <inheritdoc />
    public partial class AddNotificationIsRead : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('[Notifications]', 'IsRead') IS NULL
BEGIN
    ALTER TABLE [Notifications]
    ADD [IsRead] bit NOT NULL CONSTRAINT [DF_Notifications_IsRead] DEFAULT (0);
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF COL_LENGTH('[Notifications]', 'IsRead') IS NOT NULL
BEGIN
    DECLARE @df nvarchar(128);
    SELECT @df = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c ON c.default_object_id = dc.object_id
    WHERE dc.parent_object_id = OBJECT_ID('[Notifications]')
      AND c.name = 'IsRead';

    IF @df IS NOT NULL
        EXEC('ALTER TABLE [Notifications] DROP CONSTRAINT [' + @df + ']');

    ALTER TABLE [Notifications] DROP COLUMN [IsRead];
END
");
        }
    }
}

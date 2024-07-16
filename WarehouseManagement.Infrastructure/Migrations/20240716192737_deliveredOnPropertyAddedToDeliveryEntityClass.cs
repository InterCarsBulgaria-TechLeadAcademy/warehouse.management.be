using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deliveredOnPropertyAddedToDeliveryEntityClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Zones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 804, DateTimeKind.Utc).AddTicks(7853),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Vendors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(7736),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Markers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(4870),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Entries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(1796),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 798, DateTimeKind.Utc).AddTicks(4557),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedOn",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "DifferenceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 801, DateTimeKind.Utc).AddTicks(6048)),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DifferenceType", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DifferenceType");

            migrationBuilder.DropColumn(
                name: "ApprovedOn",
                table: "Deliveries");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Zones",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 804, DateTimeKind.Utc).AddTicks(7853));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Vendors",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(7736));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Markers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(4870));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Entries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(1796));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 798, DateTimeKind.Utc).AddTicks(4557));
        }
    }
}

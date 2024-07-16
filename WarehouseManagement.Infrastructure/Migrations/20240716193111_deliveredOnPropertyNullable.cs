using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class deliveredOnPropertyNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Zones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 605, DateTimeKind.Utc).AddTicks(2807),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 804, DateTimeKind.Utc).AddTicks(7853));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Vendors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 602, DateTimeKind.Utc).AddTicks(8545),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(7736));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Markers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 602, DateTimeKind.Utc).AddTicks(5784),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(4870));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Entries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 602, DateTimeKind.Utc).AddTicks(2582),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(1796));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "DifferenceType",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 601, DateTimeKind.Utc).AddTicks(6593),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 801, DateTimeKind.Utc).AddTicks(6048));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 597, DateTimeKind.Utc).AddTicks(8368),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 798, DateTimeKind.Utc).AddTicks(4557));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedOn",
                table: "Deliveries",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Zones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 804, DateTimeKind.Utc).AddTicks(7853),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 605, DateTimeKind.Utc).AddTicks(2807));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Vendors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(7736),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 602, DateTimeKind.Utc).AddTicks(8545));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Markers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(4870),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 602, DateTimeKind.Utc).AddTicks(5784));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Entries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 802, DateTimeKind.Utc).AddTicks(1796),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 602, DateTimeKind.Utc).AddTicks(2582));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "DifferenceType",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 801, DateTimeKind.Utc).AddTicks(6048),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 601, DateTimeKind.Utc).AddTicks(6593));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 19, 27, 36, 798, DateTimeKind.Utc).AddTicks(4557),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 19, 31, 10, 597, DateTimeKind.Utc).AddTicks(8368));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedOn",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}

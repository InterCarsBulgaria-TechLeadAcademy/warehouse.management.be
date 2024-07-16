using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class EntryTyposFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartedProccessing",
                table: "Entries",
                newName: "StartedProcessing");

            migrationBuilder.RenameColumn(
                name: "FinishedProccessing",
                table: "Entries",
                newName: "FinishedProcessing");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Zones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 846, DateTimeKind.Utc).AddTicks(4730),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Vendors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 844, DateTimeKind.Utc).AddTicks(7277),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Markers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 844, DateTimeKind.Utc).AddTicks(4656),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Entries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 844, DateTimeKind.Utc).AddTicks(1736),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 840, DateTimeKind.Utc).AddTicks(6359),
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedProcessing",
                table: "Deliveries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedProcessing",
                table: "Deliveries",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Deliveries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DifferenceType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 843, DateTimeKind.Utc).AddTicks(6443)),
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
                name: "FinishedProcessing",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "StartedProcessing",
                table: "Deliveries");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Deliveries");

            migrationBuilder.RenameColumn(
                name: "StartedProcessing",
                table: "Entries",
                newName: "StartedProccessing");

            migrationBuilder.RenameColumn(
                name: "FinishedProcessing",
                table: "Entries",
                newName: "FinishedProccessing");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Zones",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 846, DateTimeKind.Utc).AddTicks(4730));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Vendors",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 844, DateTimeKind.Utc).AddTicks(7277));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Markers",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 844, DateTimeKind.Utc).AddTicks(4656));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Entries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 844, DateTimeKind.Utc).AddTicks(1736));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Deliveries",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValue: new DateTime(2024, 7, 16, 15, 52, 26, 840, DateTimeKind.Utc).AddTicks(6359));
        }
    }
}

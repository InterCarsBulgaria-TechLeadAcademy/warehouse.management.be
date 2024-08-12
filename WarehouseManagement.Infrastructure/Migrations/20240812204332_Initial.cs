using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WarehouseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DifferenceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DifferenceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntityChanges",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChangedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityChanges", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Markers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoutePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActionName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ControllerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoutePermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    SystemNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsFinal = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zones", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleRoutePermission",
                columns: table => new
                {
                    RoutePermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleRoutePermission", x => new { x.RoleId, x.RoutePermissionId });
                    table.ForeignKey(
                        name: "FK_RoleRoutePermission_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RoleRoutePermission_RoutePermissions_RoutePermissionId",
                        column: x => x.RoutePermissionId,
                        principalTable: "RoutePermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Deliveries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SystemNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReceptionNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TruckNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Cmr = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeliveryTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApprovedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Pallets = table.Column<int>(type: "int", nullable: false),
                    Packages = table.Column<int>(type: "int", nullable: false),
                    Pieces = table.Column<int>(type: "int", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    StartedProcessing = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishedProcessing = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deliveries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Deliveries_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorsMarkers",
                columns: table => new
                {
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    MarkerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorsMarkers", x => new { x.MarkerId, x.VendorId });
                    table.ForeignKey(
                        name: "FK_VendorsMarkers_Markers_MarkerId",
                        column: x => x.MarkerId,
                        principalTable: "Markers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorsMarkers_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorsZones",
                columns: table => new
                {
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorsZones", x => new { x.ZoneId, x.VendorId });
                    table.ForeignKey(
                        name: "FK_VendorsZones_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorsZones_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ZonesMarkers",
                columns: table => new
                {
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    MarkerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZonesMarkers", x => new { x.ZoneId, x.MarkerId });
                    table.ForeignKey(
                        name: "FK_ZonesMarkers_Markers_MarkerId",
                        column: x => x.MarkerId,
                        principalTable: "Markers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ZonesMarkers_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveriesMarkers",
                columns: table => new
                {
                    DeliveryId = table.Column<int>(type: "int", nullable: false),
                    MarkerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveriesMarkers", x => new { x.MarkerId, x.DeliveryId });
                    table.ForeignKey(
                        name: "FK_DeliveriesMarkers_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DeliveriesMarkers_Markers_MarkerId",
                        column: x => x.MarkerId,
                        principalTable: "Markers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Differences",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceptionNumber = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    InternalNumber = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    ActiveNumber = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    AdminComment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Count = table.Column<int>(type: "int", maxLength: 2147483647, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false),
                    DeliveryId = table.Column<int>(type: "int", nullable: false),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Differences", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Differences_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Differences_DifferenceTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "DifferenceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Differences_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pallets = table.Column<int>(type: "int", nullable: false),
                    Packages = table.Column<int>(type: "int", nullable: false),
                    Pieces = table.Column<int>(type: "int", nullable: false),
                    StartedProcessing = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishedProcessing = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ZoneId = table.Column<int>(type: "int", nullable: false),
                    DeliveryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedByUserId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Entries_Deliveries_DeliveryId",
                        column: x => x.DeliveryId,
                        principalTable: "Deliveries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Entries_Zones_ZoneId",
                        column: x => x.ZoneId,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "RoutePermissions",
                columns: new[] { "Id", "ActionName", "ControllerName", "CreatedAt", "CreatedByUserId", "DeletedAt", "DeletedByUserId", "IsDeleted", "LastModifiedAt", "LastModifiedByUserId" },
                values: new object[,]
                {
                    { new Guid("01d3db6f-14a5-452c-acd3-70d3769461fd"), "GetVendor", "Vendor", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("0b49a008-c018-4acd-90b3-a08cfbf393c4"), "GetHistory", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("0ee63829-f237-4204-a7af-c425e6b18b72"), "Restore", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("0ef49517-dbcf-41ce-a2ac-2907faf0776f"), "GetAll", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("18252657-f04e-497f-b1e9-b3f945f563e3"), "Edit", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("1971375a-6333-4395-be4e-ecbaea15bdc0"), "Delete", "Vendor", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("1bdbf393-1308-4054-9eaf-7cd7b65ada86"), "GetById", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("1e113fe8-0b69-4be3-b783-082a291c1587"), "GetDeliveries", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("21e9ad37-7460-4e86-826e-d1e991ded705"), "AllDeleted", "Vendor", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("237aa22e-8455-44e9-a7ea-8330c1e4c0f3"), "Create", "Role", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("2b10ec34-80e8-4768-8c62-aaa3209763a2"), "GetById", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("2f125a94-e01f-4095-b8ce-e2a5ca5d2c35"), "Entries", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("3166906a-e173-4278-9175-b2ef028c2a55"), "GetDeletedMarkers", "Marker", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("36935ca8-e05c-4c95-868a-a7a54b1f6c0f"), "Add", "DifferenceType", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("37b76e9b-4735-43ce-a476-995032cb7358"), "Add", "Marker", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("39ab107e-4b06-41d8-b0d5-e4e11af17aef"), "Logout", "Auth", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("39e90d41-da2c-44e1-b8d7-ab59a8cbc66f"), "Restore", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("39f63c45-a19e-4051-9cfe-c20d48c0a8f2"), "GetAll", "Marker", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("3b14fb3b-e21e-4b40-b2ce-42e4fba44d98"), "Edit", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("3f684d24-8b41-428d-9d16-f214390b5459"), "Restore", "Marker", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("45f63579-0d4e-4113-aaaf-b3434deaea7a"), "GetById", "RoutePermission", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("49e73ff6-04d3-4beb-a1e6-75f22b74e077"), "GetDelivery", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("4e3155e9-cb1b-4056-ac5f-a06eea066b5d"), "Edit", "Role", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("5255ef9a-8d43-41bf-9c37-96331fa953d9"), "Edit", "Vendor", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("58e984cd-6941-4a78-a5e5-dae400e48b7c"), "Add", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("5961248b-38b2-48ec-afde-468d15136b43"), "NoDifferences", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("59f6c50d-bf95-4f6c-ad03-183a3c456be2"), "GetById", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("5a77c1f2-3b8e-475d-b093-fb5aa0883e9f"), "GetAll", "Role", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("645b7d2f-20af-41b2-98d4-708ba7d08a66"), "FinishProcessing", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("6946b8c2-4b9f-4785-bb1d-cae7fe149849"), "GetAllWithDeleted", "RoutePermission", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("70bf9c60-a7cf-4d57-b1e7-b192fdaa55eb"), "All", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("72a09570-26ba-4ade-96c7-e163a395f7f7"), "GetAll", "Marker", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("7598dda9-ab15-43c7-a215-a667ca4b4b1d"), "StartProcessing", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("7a69c406-a2cd-4604-a7a3-6b2df08da3e7"), "Restore", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("7f757a2f-23bf-493a-aaf0-a4daad2d2f9c"), "Delete", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("7fa1ac80-b4af-4be2-966e-00dec1b366d6"), "All", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("84938c3e-3020-400c-90ec-884d9d5c8c14"), "Delete", "RoutePermission", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("87d0104e-0cad-4d9e-a83b-c7180ea25a3c"), "AllWithDeleted", "DifferenceType", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("87db784e-83a5-4ef8-aad9-602aabff4438"), "Edit", "Marker", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("88d91cf3-8cab-4ac8-b3cb-6a8071df7985"), "Edit", "DifferenceType", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("8a5c65a5-10dd-4b9a-969b-a286d4b0cd5e"), "FinishProcessing", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("8abee68a-22e6-4795-af74-e9bae7df712d"), "SignIn", "Auth", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("8faae285-742a-4e87-8dad-9b0161149b4d"), "Add", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("8fe3e40a-8d76-460c-8e4f-93fedbc00d0f"), "All", "Vendor", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("9398c93a-7614-4209-8f66-0b9294e77f04"), "Delete", "DifferenceType", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("968d31bd-2d50-442d-be71-6dfc924af994"), "AllDeleted", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("99f9eddf-fdf3-458e-aaee-865c520eaa06"), "AllWithDeleted", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("99ffcfb0-7d10-481e-8530-d1bdf99a7f7c"), "SignUp", "Auth", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("9e0e86ee-ff0c-49a7-881f-3cd72138218d"), "AllWithDeleted", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("9f60c085-dcf9-4241-a1d2-2debc6b0ade3"), "Refresh", "Auth", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("a37336f4-01f0-4bc3-b669-c04e258c998d"), "All", "DifferenceType", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("aab5ca97-0de2-4cb7-86b3-0c5c32b91731"), "Move", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("ae1aeba5-cb4f-4fdc-9ffd-51fbe117b6ad"), "Restore", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("b3ec2088-0826-47b9-9ec8-206feb8a9ee2"), "GetByName", "Role", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("b5284213-d20a-43cc-804c-6bcc4999b8e4"), "Delete", "Marker", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("bf6e06b6-ad42-4071-90bb-02d216767d89"), "Edit", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("c1ad7c4a-faf1-4baa-94fe-ea1de779d758"), "Delete", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("c46882bc-99f9-4d72-95c6-ab30f6eac857"), "Delete", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("ca750589-fdf1-4d86-b6f9-bdda25037d81"), "GetAll", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("cd17679c-2479-4f62-bc4c-45d3985e7ce9"), "Restore", "DifferenceType", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("cdb4e120-65dd-466f-9400-3235ad59944e"), "Add", "Vendor", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("d2e32e26-95cb-4f52-88ad-3ba92255e824"), "Approve", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("d30b81f8-328e-450a-b987-27e04e7f8048"), "Restore", "Vendor", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("db0be472-ae5d-441c-8363-d6d9afb382f4"), "StartProcessing", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("e0e9774b-1244-4df7-aa45-3428ad2e58e1"), "Delete", "Role", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("e1c62105-4f16-4746-94fb-0f1b599ae760"), "AddUserToRole", "Role", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("e4a9ffd6-82b3-4884-8fdf-8b1b42b3157e"), "GetById", "DifferenceType", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("e5123e76-631b-4ea1-b6df-88002b8d5d59"), "Add", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("e5dda1f8-e431-4a86-879d-0abe4dfdcdc7"), "Delete", "Delivery", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("e852de35-9c19-4fee-ac0d-1a481faaf5e5"), "AllWithDeleted", "Zone", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("f38fe8bc-f73f-442e-8283-bb79d6aa94f7"), "Edit", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("f7771647-818d-4483-ad20-c08809835034"), "AllWithDeleted", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("fa661482-15a4-4e3d-858e-5db9811f4ed9"), "GetMarker", "Marker", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("fbda2c57-bf40-4a37-9123-d3eb94a8e5b7"), "Add", "Difference", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("fd296bbf-ac43-406c-b5d8-414535b2d686"), "GetAll", "RoutePermission", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null },
                    { new Guid("fd52508a-25e5-4174-91d1-1535ca7d3549"), "Split", "Entry", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", null, null, false, null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Deliveries_VendorId",
                table: "Deliveries",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveriesMarkers_DeliveryId",
                table: "DeliveriesMarkers",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Differences_DeliveryId",
                table: "Differences",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Differences_TypeId",
                table: "Differences",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Differences_ZoneId",
                table: "Differences",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_DeliveryId",
                table: "Entries",
                column: "DeliveryId");

            migrationBuilder.CreateIndex(
                name: "IX_Entries_ZoneId",
                table: "Entries",
                column: "ZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleRoutePermission_RoutePermissionId",
                table: "RoleRoutePermission",
                column: "RoutePermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorsMarkers_VendorId",
                table: "VendorsMarkers",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorsZones_VendorId",
                table: "VendorsZones",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ZonesMarkers_MarkerId",
                table: "ZonesMarkers",
                column: "MarkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "DeliveriesMarkers");

            migrationBuilder.DropTable(
                name: "Differences");

            migrationBuilder.DropTable(
                name: "EntityChanges");

            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "RoleRoutePermission");

            migrationBuilder.DropTable(
                name: "VendorsMarkers");

            migrationBuilder.DropTable(
                name: "VendorsZones");

            migrationBuilder.DropTable(
                name: "ZonesMarkers");

            migrationBuilder.DropTable(
                name: "DifferenceTypes");

            migrationBuilder.DropTable(
                name: "Deliveries");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "RoutePermissions");

            migrationBuilder.DropTable(
                name: "Markers");

            migrationBuilder.DropTable(
                name: "Zones");

            migrationBuilder.DropTable(
                name: "Vendors");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixForeignKeyInRoleRoutePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleRoutePermission_RoutePermissions_RoleId",
                table: "RoleRoutePermission");

            migrationBuilder.CreateIndex(
                name: "IX_RoleRoutePermission_RoutePermissionId",
                table: "RoleRoutePermission",
                column: "RoutePermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRoutePermission_RoutePermissions_RoutePermissionId",
                table: "RoleRoutePermission",
                column: "RoutePermissionId",
                principalTable: "RoutePermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RoleRoutePermission_RoutePermissions_RoutePermissionId",
                table: "RoleRoutePermission");

            migrationBuilder.DropIndex(
                name: "IX_RoleRoutePermission_RoutePermissionId",
                table: "RoleRoutePermission");

            migrationBuilder.AddForeignKey(
                name: "FK_RoleRoutePermission_RoutePermissions_RoleId",
                table: "RoleRoutePermission",
                column: "RoleId",
                principalTable: "RoutePermissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

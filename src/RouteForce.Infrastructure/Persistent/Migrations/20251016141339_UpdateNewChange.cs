using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteForce.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNewChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserType",
                table: "Users",
                newName: "UserRole");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserType",
                table: "Users",
                newName: "IX_Users_UserRole");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserRole",
                table: "Users",
                newName: "UserType");

            migrationBuilder.RenameIndex(
                name: "IX_Users_UserRole",
                table: "Users",
                newName: "IX_Users_UserType");
        }
    }
}

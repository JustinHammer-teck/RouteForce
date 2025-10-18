using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteForce.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class ADDCHANGE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkpoints_Business_ManagedByBusinessId",
                table: "Checkpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Business_BusinessId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalReceiver_Business_CreatedByBusinessId",
                table: "PersonalReceiver");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Business_BusinessId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Business",
                table: "Business");

            migrationBuilder.RenameTable(
                name: "Business",
                newName: "Businesses");

            migrationBuilder.RenameIndex(
                name: "IX_Business_IsActive",
                table: "Businesses",
                newName: "IX_Businesses_IsActive");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Businesses",
                table: "Businesses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkpoints_Businesses_ManagedByBusinessId",
                table: "Checkpoints",
                column: "ManagedByBusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Businesses_BusinessId",
                table: "Order",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalReceiver_Businesses_CreatedByBusinessId",
                table: "PersonalReceiver",
                column: "CreatedByBusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Businesses_BusinessId",
                table: "Users",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkpoints_Businesses_ManagedByBusinessId",
                table: "Checkpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Businesses_BusinessId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalReceiver_Businesses_CreatedByBusinessId",
                table: "PersonalReceiver");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Businesses_BusinessId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Businesses",
                table: "Businesses");

            migrationBuilder.RenameTable(
                name: "Businesses",
                newName: "Business");

            migrationBuilder.RenameIndex(
                name: "IX_Businesses_IsActive",
                table: "Business",
                newName: "IX_Business_IsActive");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Business",
                table: "Business",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Checkpoints_Business_ManagedByBusinessId",
                table: "Checkpoints",
                column: "ManagedByBusinessId",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Business_BusinessId",
                table: "Order",
                column: "BusinessId",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalReceiver_Business_CreatedByBusinessId",
                table: "PersonalReceiver",
                column: "CreatedByBusinessId",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Business_BusinessId",
                table: "Users",
                column: "BusinessId",
                principalTable: "Business",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

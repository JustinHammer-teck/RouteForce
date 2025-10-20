using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteForce.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class AlotOfChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Checkpoints_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                table: "Checkpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                table: "Orders");

            migrationBuilder.DropTable(
                name: "DeliveryServiceTemplate");

            migrationBuilder.DropIndex(
                name: "IX_Orders_DeliveryServiceTemplateId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Checkpoints_DeliveryServiceTemplateId",
                table: "Checkpoints");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryServiceTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    EstimatedDeliveryDays = table.Column<int>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ServiceCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryServiceTemplate", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_DeliveryServiceTemplateId",
                table: "Orders",
                column: "DeliveryServiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_DeliveryServiceTemplateId",
                table: "Checkpoints",
                column: "DeliveryServiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryServiceTemplate_IsActive",
                table: "DeliveryServiceTemplate",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryServiceTemplate_ServiceCode",
                table: "DeliveryServiceTemplate",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Checkpoints_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                table: "Checkpoints",
                column: "DeliveryServiceTemplateId",
                principalTable: "DeliveryServiceTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                table: "Orders",
                column: "DeliveryServiceTemplateId",
                principalTable: "DeliveryServiceTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

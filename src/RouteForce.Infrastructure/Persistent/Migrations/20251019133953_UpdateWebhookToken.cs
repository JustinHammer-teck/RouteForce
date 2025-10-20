using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteForce.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWebhookToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddress_PersonalReceiver_PersonalReceiverId",
                table: "DeliveryAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Order_OrderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Businesses_BusinessId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Checkpoints_DeliveryCheckpointId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryAddress_SelectedDeliveryAddressId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_PersonalReceiver_PersonalReceiverId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalReceiver_Businesses_CreatedByBusinessId",
                table: "PersonalReceiver");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteCheckpoints_Order_OrderId",
                table: "RouteCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_WebhookTokens_Order_OrderId",
                table: "WebhookTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_WebhookTokens_PersonalReceiver_IssuedToPersonalReceiverId",
                table: "WebhookTokens");

            migrationBuilder.DropIndex(
                name: "IX_WebhookTokens_IssuedToPersonalReceiverId",
                table: "WebhookTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonalReceiver",
                table: "PersonalReceiver");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryAddress",
                table: "DeliveryAddress");

            migrationBuilder.DropColumn(
                name: "IssuedToPersonalReceiverId",
                table: "WebhookTokens");

            migrationBuilder.RenameTable(
                name: "PersonalReceiver",
                newName: "PersonalReceivers");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "DeliveryAddress",
                newName: "DeliveryAddresses");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalReceiver_Email",
                table: "PersonalReceivers",
                newName: "IX_PersonalReceivers_Email");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalReceiver_CreatedByBusinessId",
                table: "PersonalReceivers",
                newName: "IX_PersonalReceivers_CreatedByBusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_TrackingNumber",
                table: "Orders",
                newName: "IX_Orders_TrackingNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Order_Status",
                table: "Orders",
                newName: "IX_Orders_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Order_SelectedDeliveryAddressId",
                table: "Orders",
                newName: "IX_Orders_SelectedDeliveryAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_PersonalReceiverId",
                table: "Orders",
                newName: "IX_Orders_PersonalReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DeliveryServiceTemplateId",
                table: "Orders",
                newName: "IX_Orders_DeliveryServiceTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_DeliveryCheckpointId",
                table: "Orders",
                newName: "IX_Orders_DeliveryCheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CreatedDate",
                table: "Orders",
                newName: "IX_Orders_CreatedDate");

            migrationBuilder.RenameIndex(
                name: "IX_Order_BusinessId_Status",
                table: "Orders",
                newName: "IX_Orders_BusinessId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddress_PersonalReceiverId_IsDefault",
                table: "DeliveryAddresses",
                newName: "IX_DeliveryAddresses_PersonalReceiverId_IsDefault");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddress_PersonalReceiverId",
                table: "DeliveryAddresses",
                newName: "IX_DeliveryAddresses_PersonalReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddress_IsActive",
                table: "DeliveryAddresses",
                newName: "IX_DeliveryAddresses_IsActive");

            migrationBuilder.AddColumn<string>(
                name: "IssueType",
                table: "WebhookTokens",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonalReceivers",
                table: "PersonalReceivers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryAddresses",
                table: "DeliveryAddresses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookTokens_IssueType",
                table: "WebhookTokens",
                column: "IssueType");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddresses_PersonalReceivers_PersonalReceiverId",
                table: "DeliveryAddresses",
                column: "PersonalReceiverId",
                principalTable: "PersonalReceivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Orders_OrderId",
                table: "Notifications",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Businesses_BusinessId",
                table: "Orders",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Checkpoints_DeliveryCheckpointId",
                table: "Orders",
                column: "DeliveryCheckpointId",
                principalTable: "Checkpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryAddresses_SelectedDeliveryAddressId",
                table: "Orders",
                column: "SelectedDeliveryAddressId",
                principalTable: "DeliveryAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                table: "Orders",
                column: "DeliveryServiceTemplateId",
                principalTable: "DeliveryServiceTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_PersonalReceivers_PersonalReceiverId",
                table: "Orders",
                column: "PersonalReceiverId",
                principalTable: "PersonalReceivers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonalReceivers_Businesses_CreatedByBusinessId",
                table: "PersonalReceivers",
                column: "CreatedByBusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RouteCheckpoints_Orders_OrderId",
                table: "RouteCheckpoints",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebhookTokens_Orders_OrderId",
                table: "WebhookTokens",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryAddresses_PersonalReceivers_PersonalReceiverId",
                table: "DeliveryAddresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Orders_OrderId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Businesses_BusinessId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Checkpoints_DeliveryCheckpointId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryAddresses_SelectedDeliveryAddressId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_PersonalReceivers_PersonalReceiverId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_PersonalReceivers_Businesses_CreatedByBusinessId",
                table: "PersonalReceivers");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteCheckpoints_Orders_OrderId",
                table: "RouteCheckpoints");

            migrationBuilder.DropForeignKey(
                name: "FK_WebhookTokens_Orders_OrderId",
                table: "WebhookTokens");

            migrationBuilder.DropIndex(
                name: "IX_WebhookTokens_IssueType",
                table: "WebhookTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonalReceivers",
                table: "PersonalReceivers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DeliveryAddresses",
                table: "DeliveryAddresses");

            migrationBuilder.DropColumn(
                name: "IssueType",
                table: "WebhookTokens");

            migrationBuilder.RenameTable(
                name: "PersonalReceivers",
                newName: "PersonalReceiver");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameTable(
                name: "DeliveryAddresses",
                newName: "DeliveryAddress");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalReceivers_Email",
                table: "PersonalReceiver",
                newName: "IX_PersonalReceiver_Email");

            migrationBuilder.RenameIndex(
                name: "IX_PersonalReceivers_CreatedByBusinessId",
                table: "PersonalReceiver",
                newName: "IX_PersonalReceiver_CreatedByBusinessId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_TrackingNumber",
                table: "Order",
                newName: "IX_Order_TrackingNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_Status",
                table: "Order",
                newName: "IX_Order_Status");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_SelectedDeliveryAddressId",
                table: "Order",
                newName: "IX_Order_SelectedDeliveryAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_PersonalReceiverId",
                table: "Order",
                newName: "IX_Order_PersonalReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DeliveryServiceTemplateId",
                table: "Order",
                newName: "IX_Order_DeliveryServiceTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_DeliveryCheckpointId",
                table: "Order",
                newName: "IX_Order_DeliveryCheckpointId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CreatedDate",
                table: "Order",
                newName: "IX_Order_CreatedDate");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_BusinessId_Status",
                table: "Order",
                newName: "IX_Order_BusinessId_Status");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddresses_PersonalReceiverId_IsDefault",
                table: "DeliveryAddress",
                newName: "IX_DeliveryAddress_PersonalReceiverId_IsDefault");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddresses_PersonalReceiverId",
                table: "DeliveryAddress",
                newName: "IX_DeliveryAddress_PersonalReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_DeliveryAddresses_IsActive",
                table: "DeliveryAddress",
                newName: "IX_DeliveryAddress_IsActive");

            migrationBuilder.AddColumn<int>(
                name: "IssuedToPersonalReceiverId",
                table: "WebhookTokens",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonalReceiver",
                table: "PersonalReceiver",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DeliveryAddress",
                table: "DeliveryAddress",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookTokens_IssuedToPersonalReceiverId",
                table: "WebhookTokens",
                column: "IssuedToPersonalReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryAddress_PersonalReceiver_PersonalReceiverId",
                table: "DeliveryAddress",
                column: "PersonalReceiverId",
                principalTable: "PersonalReceiver",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Order_OrderId",
                table: "Notifications",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Businesses_BusinessId",
                table: "Order",
                column: "BusinessId",
                principalTable: "Businesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Checkpoints_DeliveryCheckpointId",
                table: "Order",
                column: "DeliveryCheckpointId",
                principalTable: "Checkpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryAddress_SelectedDeliveryAddressId",
                table: "Order",
                column: "SelectedDeliveryAddressId",
                principalTable: "DeliveryAddress",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                table: "Order",
                column: "DeliveryServiceTemplateId",
                principalTable: "DeliveryServiceTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_PersonalReceiver_PersonalReceiverId",
                table: "Order",
                column: "PersonalReceiverId",
                principalTable: "PersonalReceiver",
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
                name: "FK_RouteCheckpoints_Order_OrderId",
                table: "RouteCheckpoints",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebhookTokens_Order_OrderId",
                table: "WebhookTokens",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WebhookTokens_PersonalReceiver_IssuedToPersonalReceiverId",
                table: "WebhookTokens",
                column: "IssuedToPersonalReceiverId",
                principalTable: "PersonalReceiver",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

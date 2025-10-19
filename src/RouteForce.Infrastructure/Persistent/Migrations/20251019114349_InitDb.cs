using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RouteForce.Infrastructure.Persistent.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AddressLine = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryServiceTemplate",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EstimatedDeliveryDays = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceCode = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryServiceTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonalReceiver",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedByBusinessId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalReceiver", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonalReceiver_Businesses_CreatedByBusinessId",
                        column: x => x.CreatedByBusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", maxLength: 450, nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Password = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    UserRole = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BusinessId = table.Column<int>(type: "INTEGER", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Checkpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AddressLine = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: true),
                    ContactName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    ContactPhone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ContactEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CheckpointType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ManagedByBusinessId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeliveryServiceTemplateId = table.Column<int>(type: "INTEGER", nullable: true),
                    RequiresConfirmation = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Checkpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Checkpoints_Businesses_ManagedByBusinessId",
                        column: x => x.ManagedByBusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Checkpoints_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                        column: x => x.DeliveryServiceTemplateId,
                        principalTable: "DeliveryServiceTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryAddress",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PersonalReceiverId = table.Column<int>(type: "INTEGER", nullable: false),
                    Label = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    AddressLine = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    City = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    State = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Country = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: true),
                    Longitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: true),
                    IsDefault = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryAddress_PersonalReceiver_PersonalReceiverId",
                        column: x => x.PersonalReceiverId,
                        principalTable: "PersonalReceiver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Order",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrackingNumber = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BusinessId = table.Column<int>(type: "INTEGER", nullable: false),
                    PersonalReceiverId = table.Column<int>(type: "INTEGER", nullable: false),
                    DeliveryServiceTemplateId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeliveryAddressLine = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    DeliveryCity = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DeliveryState = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DeliveryPostalCode = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DeliveryCountry = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DeliveryLatitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: true),
                    DeliveryLongitude = table.Column<decimal>(type: "TEXT", precision: 9, scale: 6, nullable: true),
                    SelectedDeliveryAddressId = table.Column<int>(type: "INTEGER", nullable: true),
                    DeliveryCheckpointId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    EstimatedDeliveryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualDeliveryDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    ProductReferenceId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Order", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Order_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_Checkpoints_DeliveryCheckpointId",
                        column: x => x.DeliveryCheckpointId,
                        principalTable: "Checkpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Order_DeliveryAddress_SelectedDeliveryAddressId",
                        column: x => x.SelectedDeliveryAddressId,
                        principalTable: "DeliveryAddress",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Order_DeliveryServiceTemplate_DeliveryServiceTemplateId",
                        column: x => x.DeliveryServiceTemplateId,
                        principalTable: "DeliveryServiceTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Order_PersonalReceiver_PersonalReceiverId",
                        column: x => x.PersonalReceiverId,
                        principalTable: "PersonalReceiver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    RecipientEmail = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    RecipientPhone = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    WebhookConfirmationUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DeliveryMethod = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    SentDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Message = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    ErrorMessage = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WebhookTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TokenValue = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TokenType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    IssuedToType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    IssuedToPersonalReceiverId = table.Column<int>(type: "INTEGER", nullable: true),
                    ExpirationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    UsageLimit = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 1),
                    UsedCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    LastUsedDate = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebhookTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WebhookTokens_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WebhookTokens_PersonalReceiver_IssuedToPersonalReceiverId",
                        column: x => x.IssuedToPersonalReceiverId,
                        principalTable: "PersonalReceiver",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RouteCheckpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OrderId = table.Column<int>(type: "INTEGER", nullable: false),
                    CheckpointId = table.Column<int>(type: "INTEGER", nullable: false),
                    SequenceNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    ConfirmedByTokenId = table.Column<int>(type: "INTEGER", nullable: true),
                    ConfirmationTimestamp = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpectedArrival = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ActualArrival = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouteCheckpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouteCheckpoints_Checkpoints_CheckpointId",
                        column: x => x.CheckpointId,
                        principalTable: "Checkpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RouteCheckpoints_Order_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RouteCheckpoints_WebhookTokens_ConfirmedByTokenId",
                        column: x => x.ConfirmedByTokenId,
                        principalTable: "WebhookTokens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_IsActive",
                table: "Businesses",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_CheckpointType",
                table: "Checkpoints",
                column: "CheckpointType");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_DeliveryServiceTemplateId",
                table: "Checkpoints",
                column: "DeliveryServiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_IsActive",
                table: "Checkpoints",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Checkpoints_ManagedByBusinessId",
                table: "Checkpoints",
                column: "ManagedByBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddress_IsActive",
                table: "DeliveryAddress",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddress_PersonalReceiverId",
                table: "DeliveryAddress",
                column: "PersonalReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryAddress_PersonalReceiverId_IsDefault",
                table: "DeliveryAddress",
                columns: new[] { "PersonalReceiverId", "IsDefault" });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryServiceTemplate_IsActive",
                table: "DeliveryServiceTemplate",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryServiceTemplate_ServiceCode",
                table: "DeliveryServiceTemplate",
                column: "ServiceCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_CreatedDate",
                table: "Notifications",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_OrderId",
                table: "Notifications",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_Status",
                table: "Notifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Order_BusinessId_Status",
                table: "Order",
                columns: new[] { "BusinessId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Order_CreatedDate",
                table: "Order",
                column: "CreatedDate");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryCheckpointId",
                table: "Order",
                column: "DeliveryCheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_DeliveryServiceTemplateId",
                table: "Order",
                column: "DeliveryServiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_PersonalReceiverId",
                table: "Order",
                column: "PersonalReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_SelectedDeliveryAddressId",
                table: "Order",
                column: "SelectedDeliveryAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_Status",
                table: "Order",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Order_TrackingNumber",
                table: "Order",
                column: "TrackingNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonalReceiver_CreatedByBusinessId",
                table: "PersonalReceiver",
                column: "CreatedByBusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalReceiver_Email",
                table: "PersonalReceiver",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_RouteCheckpoints_CheckpointId",
                table: "RouteCheckpoints",
                column: "CheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteCheckpoints_ConfirmedByTokenId",
                table: "RouteCheckpoints",
                column: "ConfirmedByTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteCheckpoints_OrderId",
                table: "RouteCheckpoints",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_RouteCheckpoints_OrderId_SequenceNumber",
                table: "RouteCheckpoints",
                columns: new[] { "OrderId", "SequenceNumber" });

            migrationBuilder.CreateIndex(
                name: "IX_RouteCheckpoints_Status",
                table: "RouteCheckpoints",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BusinessId",
                table: "Users",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserRole",
                table: "Users",
                column: "UserRole");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookTokens_ExpirationDate",
                table: "WebhookTokens",
                column: "ExpirationDate");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookTokens_IsActive",
                table: "WebhookTokens",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookTokens_IssuedToPersonalReceiverId",
                table: "WebhookTokens",
                column: "IssuedToPersonalReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_WebhookTokens_OrderId",
                table: "WebhookTokens",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "RouteCheckpoints");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "WebhookTokens");

            migrationBuilder.DropTable(
                name: "Order");

            migrationBuilder.DropTable(
                name: "Checkpoints");

            migrationBuilder.DropTable(
                name: "DeliveryAddress");

            migrationBuilder.DropTable(
                name: "DeliveryServiceTemplate");

            migrationBuilder.DropTable(
                name: "PersonalReceiver");

            migrationBuilder.DropTable(
                name: "Businesses");
        }
    }
}

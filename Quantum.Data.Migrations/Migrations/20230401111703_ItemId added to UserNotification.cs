using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quantum.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class ItemIdaddedtoUserNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "NotificationId",
                table: "UserNotifications",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ItemID",
                table: "UserNotifications",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_ItemID",
                table: "UserNotifications",
                column: "ItemID");

            migrationBuilder.CreateIndex(
                name: "IX_UserNotifications_NotificationId",
                table: "UserNotifications",
                column: "NotificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserNotifications_Items_ItemID",
                table: "UserNotifications",
                column: "ItemID",
                principalTable: "Items",
                principalColumn: "ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserNotifications_Items_ItemID",
                table: "UserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_UserNotifications_ItemID",
                table: "UserNotifications");

            migrationBuilder.DropIndex(
                name: "IX_UserNotifications_NotificationId",
                table: "UserNotifications");

            migrationBuilder.DropColumn(
                name: "ItemID",
                table: "UserNotifications");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationId",
                table: "UserNotifications",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}

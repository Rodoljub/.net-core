using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quantum.Data.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class Item_EnableDisable_Comments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EnableComments",
                table: "Items",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnableComments",
                table: "Items");
        }
    }
}

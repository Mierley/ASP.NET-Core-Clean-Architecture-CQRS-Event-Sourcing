using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.PublicApi.Migrations
{
    /// <inheritdoc />
    public partial class _1232 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "EventStores",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "EventStores");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoMed_Backend.Migrations
{
    /// <inheritdoc />
    public partial class first_migrationv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "orders",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "orders",
                table: "Orders");
        }
    }
}

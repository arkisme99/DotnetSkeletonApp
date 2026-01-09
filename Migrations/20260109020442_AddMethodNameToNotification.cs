using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetSkeletonApp.Migrations
{
    /// <inheritdoc />
    public partial class AddMethodNameToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MethodName",
                table: "Notifications",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MethodName",
                table: "Notifications");
        }
    }
}

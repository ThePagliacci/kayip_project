using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace kayip_project.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserToPostsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ApplicationUserId",
                value: "d5634d7e-4f17-4261-900f-bb33d4f21633");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2,
                column: "ApplicationUserId",
                value: "d5634d7e-4f17-4261-900f-bb33d4f21633");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3,
                column: "ApplicationUserId",
                value: "d5634d7e-4f17-4261-900f-bb33d4f21633");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 4,
                column: "ApplicationUserId",
                value: "d5634d7e-4f17-4261-900f-bb33d4f21633");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1,
                column: "ApplicationUserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2,
                column: "ApplicationUserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3,
                column: "ApplicationUserId",
                value: "");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 4,
                column: "ApplicationUserId",
                value: "");
        }
    }
}

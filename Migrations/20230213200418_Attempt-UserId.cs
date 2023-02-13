using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class AttemptUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7f1afe32-4687-4244-b70e-4060e128174c", "AQAAAAIAAYagAAAAENi8GPXdWLXP5RnOuBVf0cqAKCMEa6spgeKyaGVSZqxpdRvpC+SaCFAIKAXaDCzuTQ==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "29d57aec-ab10-4931-9fd6-5714b41380ce", "AQAAAAIAAYagAAAAEBsddw4x1O1MvP4wW91bGWb9FzSTm31IBvl2ppXISX79cDwrC1K4losi3fo0sWLtmA==" });
        }
    }
}

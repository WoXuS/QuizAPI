using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class AttemptQuizCopy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuizCopy",
                table: "Attempts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "8f6ae197-8ece-4dd3-96f0-57c79b3e8ea6", "AQAAAAIAAYagAAAAEGP9Fnyo8iW/uFqLkpZFZ71TsBZxyD25csKpFJejKD1IQRRcvcBcqmxQxHvqUwnONw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuizCopy",
                table: "Attempts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "7f1afe32-4687-4244-b70e-4060e128174c", "AQAAAAIAAYagAAAAENi8GPXdWLXP5RnOuBVf0cqAKCMEa6spgeKyaGVSZqxpdRvpC+SaCFAIKAXaDCzuTQ==" });
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class QuizCopy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "353540b6-5d7d-44a1-b5d9-add1ce41267d", "AQAAAAIAAYagAAAAEJ6BIHZqgLCklcYW9qqIQCdpeWOo0ilRi0x80Z+5JZO5VpfI84zJ+I8L4GwedqpgPg==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "8f6ae197-8ece-4dd3-96f0-57c79b3e8ea6", "AQAAAAIAAYagAAAAEGP9Fnyo8iW/uFqLkpZFZ71TsBZxyD25csKpFJejKD1IQRRcvcBcqmxQxHvqUwnONw==" });
        }
    }
}

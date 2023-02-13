using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class QuestionQuizId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "29d57aec-ab10-4931-9fd6-5714b41380ce", "AQAAAAIAAYagAAAAEBsddw4x1O1MvP4wW91bGWb9FzSTm31IBvl2ppXISX79cDwrC1K4losi3fo0sWLtmA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "0d506196-821a-44e9-bf27-94e54f3475af", "AQAAAAIAAYagAAAAEJcs6moHiN3Br5qYrwS1lEe2mcP8Zlajzr8HHTMklS9iw95fVLCP3N4OKkW2MQ6jtQ==" });
        }
    }
}

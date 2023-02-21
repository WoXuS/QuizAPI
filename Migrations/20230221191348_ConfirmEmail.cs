using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class ConfirmEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "PasswordHash" },
                values: new object[] { "0e27e8d2-f602-45c3-b18d-5044fe13996d", true, "AQAAAAIAAYagAAAAEJMTrtlUi9fFtTQODb4egqUMAT+so0oc8Wr2d6guGSvoCIY2geQAQNfg+7JMY5qZQA==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "EmailConfirmed", "PasswordHash" },
                values: new object[] { "40ac0f1b-e939-4f59-949d-8d924db8edbd", false, "AQAAAAIAAYagAAAAEEKZquajqlE8AJQMk9/L4XST+AS/2Qsz34GhSCBgbZ4i/eZSDeU6fiKKO6Tag07mCg==" });
        }
    }
}

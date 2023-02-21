using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class mssqlmigration458 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "36676eee-eb08-4d24-9df5-68dcb7cb952d", "AQAAAAIAAYagAAAAELa2ahtiREEvEKblHQ6w6vCzQ2XhTMYya6FV0tDDsreEZtJeuF4uUL2IvIhh009SFw==" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "40ac0f1b-e939-4f59-949d-8d924db8edbd", "AQAAAAIAAYagAAAAEEKZquajqlE8AJQMk9/L4XST+AS/2Qsz34GhSCBgbZ4i/eZSDeU6fiKKO6Tag07mCg==" });
        }
    }
}

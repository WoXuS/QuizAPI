using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuizAPI.Migrations
{
    /// <inheritdoc />
    public partial class Result : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResultId",
                table: "Attempts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Submitted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChosenCorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    ChosenIncorrectAnswers = table.Column<int>(type: "int", nullable: false),
                    AllCorrectAnswers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "8b0b2f5b-e16d-4e4c-9750-ecc780769580", "AQAAAAIAAYagAAAAENuwsVAWNRmUeFRhdWBj4Ebrsl1cOUI71i+E3/Gv9PRPdnSuYlbVeXJy7gQi8kUHmg==" });

            migrationBuilder.CreateIndex(
                name: "IX_Attempts_ResultId",
                table: "Attempts",
                column: "ResultId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attempts_Results_ResultId",
                table: "Attempts",
                column: "ResultId",
                principalTable: "Results",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attempts_Results_ResultId",
                table: "Attempts");

            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Attempts_ResultId",
                table: "Attempts");

            migrationBuilder.DropColumn(
                name: "ResultId",
                table: "Attempts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "b5818efb-f28f-4486-a784-9f1eb44f51e1",
                columns: new[] { "ConcurrencyStamp", "PasswordHash" },
                values: new object[] { "353540b6-5d7d-44a1-b5d9-add1ce41267d", "AQAAAAIAAYagAAAAEJ6BIHZqgLCklcYW9qqIQCdpeWOo0ilRi0x80Z+5JZO5VpfI84zJ+I8L4GwedqpgPg==" });
        }
    }
}

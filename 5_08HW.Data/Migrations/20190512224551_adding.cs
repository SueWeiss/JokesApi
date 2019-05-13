using Microsoft.EntityFrameworkCore.Migrations;

namespace _5_08HW.Data.Migrations
{
    public partial class adding : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "punchline",
                table: "Jokes",
                newName: "Punchline");

            migrationBuilder.AddColumn<int>(
                name: "OriginId",
                table: "Jokes",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OriginId",
                table: "Jokes");

            migrationBuilder.RenameColumn(
                name: "Punchline",
                table: "Jokes",
                newName: "punchline");
        }
    }
}

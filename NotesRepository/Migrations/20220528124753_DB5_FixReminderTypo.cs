using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesRepository.Migrations
{
    public partial class DB5_FixReminderTypo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RemainderAt",
                table: "Event",
                newName: "ReminderAt");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReminderAt",
                table: "Event",
                newName: "RemainderAt");
        }
    }
}

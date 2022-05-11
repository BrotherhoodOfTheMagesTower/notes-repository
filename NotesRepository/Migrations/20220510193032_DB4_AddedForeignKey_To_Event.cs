using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesRepository.Migrations
{
    public partial class DB4_AddedForeignKey_To_Event : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Note_EventId",
                table: "Event");

            migrationBuilder.AddColumn<Guid>(
                name: "NoteId",
                table: "Event",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Event_NoteId",
                table: "Event",
                column: "NoteId",
                unique: true,
                filter: "[NoteId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Note_NoteId",
                table: "Event",
                column: "NoteId",
                principalTable: "Note",
                principalColumn: "NoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Event_Note_NoteId",
                table: "Event");

            migrationBuilder.DropIndex(
                name: "IX_Event_NoteId",
                table: "Event");

            migrationBuilder.DropColumn(
                name: "NoteId",
                table: "Event");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Note_EventId",
                table: "Event",
                column: "EventId",
                principalTable: "Note",
                principalColumn: "NoteId");
        }
    }
}

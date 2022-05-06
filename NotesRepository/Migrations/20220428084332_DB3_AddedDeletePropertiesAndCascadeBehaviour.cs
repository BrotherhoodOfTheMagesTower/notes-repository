using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotesRepository.Migrations
{
    public partial class DB3_AddedDeletePropertiesAndCascadeBehaviour : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directory_User_UserId",
                table: "Directory");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Note",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Directory",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMarkedAsDeleted",
                table: "Directory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Directory_User_UserId",
                table: "Directory",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directory_User_UserId",
                table: "Directory");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Directory");

            migrationBuilder.DropColumn(
                name: "IsMarkedAsDeleted",
                table: "Directory");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DeletedAt",
                table: "Note",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Directory_User_UserId",
                table: "Directory",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}

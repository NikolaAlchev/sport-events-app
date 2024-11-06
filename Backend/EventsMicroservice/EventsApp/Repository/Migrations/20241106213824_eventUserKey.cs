using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repository.Migrations
{
    /// <inheritdoc />
    public partial class eventUserKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUsers_AspNetUsers_UserId",
                table: "EventUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUsers",
                table: "EventUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EventUsers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "EventUsers",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUsers",
                table: "EventUsers",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUsers",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUsers_AspNetUsers_UserId",
                table: "EventUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EventUsers_AspNetUsers_UserId",
                table: "EventUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EventUsers",
                table: "EventUsers");

            migrationBuilder.DropIndex(
                name: "IX_EventUsers_EventId",
                table: "EventUsers");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "EventUsers",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EventId",
                table: "EventUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EventUsers",
                table: "EventUsers",
                columns: new[] { "EventId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_EventUsers_AspNetUsers_UserId",
                table: "EventUsers",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EventUsers_Events_EventId",
                table: "EventUsers",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

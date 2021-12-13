using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BackendAPI.Migrations
{
    public partial class AddMessageEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    SenderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    SenderUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    SenderUsername = table.Column<string>(type: "TEXT", nullable: true),
                    RecipientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipientUserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    RecipientUsername = table.Column<string>(type: "TEXT", nullable: true),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    DateRead = table.Column<DateTime>(type: "TEXT", nullable: true),
                    MessageSent = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SenderDeleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    RecipientDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_AppUsers_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_AppUsers_SenderUserId",
                        column: x => x.SenderUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_RecipientUserId",
                table: "Messages",
                column: "RecipientUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_SenderUserId",
                table: "Messages",
                column: "SenderUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Messages");
        }
    }
}

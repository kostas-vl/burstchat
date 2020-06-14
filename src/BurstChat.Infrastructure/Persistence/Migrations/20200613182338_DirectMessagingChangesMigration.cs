using Microsoft.EntityFrameworkCore.Migrations;

namespace BurstChat.Infrastructure.Persistence.Migrations
{
    public partial class DirectMessagingChangesMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DirectMessaging_FirstParticipantUserId",
                table: "DirectMessaging",
                column: "FirstParticipantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectMessaging_SecondParticipantUserId",
                table: "DirectMessaging",
                column: "SecondParticipantUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessaging_Users_FirstParticipantUserId",
                table: "DirectMessaging",
                column: "FirstParticipantUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DirectMessaging_Users_SecondParticipantUserId",
                table: "DirectMessaging",
                column: "SecondParticipantUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessaging_Users_FirstParticipantUserId",
                table: "DirectMessaging");

            migrationBuilder.DropForeignKey(
                name: "FK_DirectMessaging_Users_SecondParticipantUserId",
                table: "DirectMessaging");

            migrationBuilder.DropIndex(
                name: "IX_DirectMessaging_FirstParticipantUserId",
                table: "DirectMessaging");

            migrationBuilder.DropIndex(
                name: "IX_DirectMessaging_SecondParticipantUserId",
                table: "DirectMessaging");
        }
    }
}

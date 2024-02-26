using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BurstChat.Infrastructure.Persistence.Migrations
{
    public partial class RemovedTheSipModelMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Users_Sip_SipId", table: "Users");

            migrationBuilder.DropTable(name: "Sip");

            migrationBuilder.DropIndex(name: "IX_Users_SipId", table: "Users");

            migrationBuilder.DropColumn(name: "SipId", table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SipId",
                table: "Users",
                type: "bigint",
                nullable: false,
                defaultValue: 0L
            );

            migrationBuilder.CreateTable(
                name: "Sip",
                columns: table => new
                {
                    Id = table
                        .Column<long>(type: "bigint", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Username = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sip", x => x.Id);
                }
            );

            migrationBuilder.CreateIndex(name: "IX_Users_SipId", table: "Users", column: "SipId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Sip_SipId",
                table: "Users",
                column: "SipId",
                principalTable: "Sip",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationsAPI.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserEmail = table.Column<string>(type: "VARCHAR(512)", nullable: false),
                    Subject = table.Column<string>(type: "VARCHAR(512)", nullable: false),
                    Body = table.Column<string>(type: "VARCHAR(MAX)", nullable: false),
                    Type = table.Column<int>(type: "INT", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "DATETIME", nullable: false),
                    SentAt = table.Column<DateTime>(type: "DATETIME", nullable: true),
                    ErrorMessage = table.Column<string>(type: "VARCHAR(1024)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");
        }
    }
}

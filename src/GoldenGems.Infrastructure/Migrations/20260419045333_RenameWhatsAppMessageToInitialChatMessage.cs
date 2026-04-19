using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldenGems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameWhatsAppMessageToInitialChatMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "WhatsAppMessage",
                table: "Products",
                newName: "InitialChatMessage");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InitialChatMessage",
                table: "Products",
                newName: "WhatsAppMessage");
        }
    }
}

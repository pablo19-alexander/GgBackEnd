using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldenGems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoUrlToPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "People",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "People");
        }
    }
}

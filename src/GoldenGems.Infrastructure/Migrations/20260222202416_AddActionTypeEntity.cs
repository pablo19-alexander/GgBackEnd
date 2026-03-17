using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoldenGems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddActionTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "Actions");

            migrationBuilder.AddColumn<Guid>(
                name: "ActionTypeId",
                table: "Actions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ActionTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActionTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ActionTypeId",
                table: "Actions",
                column: "ActionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ActionTypes_Code",
                table: "ActionTypes",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_ActionTypes_ActionTypeId",
                table: "Actions",
                column: "ActionTypeId",
                principalTable: "ActionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_ActionTypes_ActionTypeId",
                table: "Actions");

            migrationBuilder.DropTable(
                name: "ActionTypes");

            migrationBuilder.DropIndex(
                name: "IX_Actions_ActionTypeId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "ActionTypeId",
                table: "Actions");

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                table: "Actions",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}

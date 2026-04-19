using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GoldenGems.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedActionTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ActionTypes",
                columns: new[] { "Id", "Code", "CreatedAt", "Description", "IsActive", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-000000000001"), "READ", new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Operaciones de lectura o consulta", true, null },
                    { new Guid("11111111-1111-1111-1111-000000000002"), "WRITE", new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Operaciones de creación", true, null },
                    { new Guid("11111111-1111-1111-1111-000000000003"), "UPDATE", new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Operaciones de edición o actualización", true, null },
                    { new Guid("11111111-1111-1111-1111-000000000004"), "DELETE", new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Operaciones de eliminación", true, null },
                    { new Guid("11111111-1111-1111-1111-000000000005"), "EXPORT", new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Exportación de datos", true, null },
                    { new Guid("11111111-1111-1111-1111-000000000006"), "EXECUTE", new DateTime(2026, 4, 18, 0, 0, 0, 0, DateTimeKind.Utc), "Ejecución de procesos", true, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ActionTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-000000000001"));

            migrationBuilder.DeleteData(
                table: "ActionTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-000000000002"));

            migrationBuilder.DeleteData(
                table: "ActionTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-000000000003"));

            migrationBuilder.DeleteData(
                table: "ActionTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-000000000004"));

            migrationBuilder.DeleteData(
                table: "ActionTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-000000000005"));

            migrationBuilder.DeleteData(
                table: "ActionTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-000000000006"));
        }
    }
}

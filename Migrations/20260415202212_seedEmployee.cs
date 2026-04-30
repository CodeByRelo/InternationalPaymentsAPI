using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InternationalPaymentsAPI.Migrations
{
    /// <inheritdoc />
    public partial class seedEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "AccountNumber", "CreatedAt", "FullName", "IdNumber", "PasswordHash", "Role" },
                values: new object[] { 99, "EMP001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System Admin", "0000000000000", "ADMIN123", "Employee" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 99);
        }
    }
}

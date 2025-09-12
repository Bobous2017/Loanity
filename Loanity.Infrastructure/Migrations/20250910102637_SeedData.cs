using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Loanity.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EquipmentCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Laptop" },
                    { 2, "Tablet" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "Equipment",
                columns: new[] { "Id", "CategoryId", "Color", "Name", "QrCode", "SerialNumber", "Status" },
                values: new object[,]
                {
                    { 1, 1, null, "Dell XPS 13", "QR123", "SN123", "Available" },
                    { 2, 2, null, "iPad Pro", "QR124", "SN124", "Available" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "Phone", "RoleId" },
                values: new object[,]
                {
                    { 1, "alice@example.com", "Alice", "Admin", null, 1 },
                    { 2, "bob@example.com", "Bob", "Borrower", null, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Equipment",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EquipmentCategories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}

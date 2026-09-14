using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MaintenanceDesk.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedReferenceData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Properties",
                columns: new[] { "Id", "City", "Name", "PostalCode", "StreetAddress" },
                values: new object[,]
                {
                    { new Guid("10000000-0000-0000-0000-000000000001"), "Uppsala", "Tallbacken", "752 36", "Tallbacksvägen 12" },
                    { new Guid("10000000-0000-0000-0000-000000000002"), "Malmö", "Hamnhuset", "211 22", "Hamngatan 8" }
                });

            migrationBuilder.InsertData(
                table: "Technicians",
                columns: new[] { "Id", "Email", "FullName", "Trade" },
                values: new object[,]
                {
                    { new Guid("40000000-0000-0000-0000-000000000001"), "johan.berg@example.com", "Johan Berg", "Plumber" },
                    { new Guid("40000000-0000-0000-0000-000000000002"), "maria.holm@example.com", "Maria Holm", "Electrician" },
                    { new Guid("40000000-0000-0000-0000-000000000003"), "lars.ek@example.com", "Lars Ek", "Caretaker" }
                });

            migrationBuilder.InsertData(
                table: "Units",
                columns: new[] { "Id", "Designation", "Floor", "PropertyId" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), "1001", 0, new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000002"), "1101", 1, new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000003"), "1201", 2, new Guid("10000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000004"), "1001", 0, new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-000000000005"), "1102", 1, new Guid("10000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-000000000006"), "1301", 3, new Guid("10000000-0000-0000-0000-000000000002") }
                });

            migrationBuilder.InsertData(
                table: "Residents",
                columns: new[] { "Id", "Email", "FullName", "PhoneNumber", "UnitId" },
                values: new object[,]
                {
                    { new Guid("30000000-0000-0000-0000-000000000001"), "anna.lindqvist@example.com", "Anna Lindqvist", "070-1740605", new Guid("20000000-0000-0000-0000-000000000002") },
                    { new Guid("30000000-0000-0000-0000-000000000002"), "erik.johansson@example.com", "Erik Johansson", "070-1740612", new Guid("20000000-0000-0000-0000-000000000003") },
                    { new Guid("30000000-0000-0000-0000-000000000003"), "sara.nilsson@example.com", "Sara Nilsson", null, new Guid("20000000-0000-0000-0000-000000000004") },
                    { new Guid("30000000-0000-0000-0000-000000000004"), "omar.haddad@example.com", "Omar Haddad", "070-1740623", new Guid("20000000-0000-0000-0000-000000000006") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Residents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Residents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Residents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Residents",
                keyColumn: "Id",
                keyValue: new Guid("30000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Technicians",
                keyColumn: "Id",
                keyValue: new Guid("40000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Units",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Properties",
                keyColumn: "Id",
                keyValue: new Guid("10000000-0000-0000-0000-000000000002"));
        }
    }
}

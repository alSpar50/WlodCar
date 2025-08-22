using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WlodCar.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceFieldsToCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "InService",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastServiceDate",
                table: "Cars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextServiceDate",
                table: "Cars",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ServiceNotes",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InService",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "LastServiceDate",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "NextServiceDate",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "ServiceNotes",
                table: "Cars");
        }
    }
}

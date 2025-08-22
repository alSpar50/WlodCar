using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WlodCar.Migrations
{
    /// <inheritdoc />
    public partial class AddImageFileNameToCar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageFileName",
                table: "Cars",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageFileName",
                table: "Cars");
        }
    }
}

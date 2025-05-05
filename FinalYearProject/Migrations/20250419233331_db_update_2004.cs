using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Migrations
{
    /// <inheritdoc />
    public partial class db_update_2004 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColourCode",
                table: "ProductVariant",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Product",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Path",
                table: "Category",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    inv_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    order_status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    despatch_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    inv_firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    inv_surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    tracking_url = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropColumn(
                name: "ColourCode",
                table: "ProductVariant");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Path",
                table: "Category");
        }
    }
}

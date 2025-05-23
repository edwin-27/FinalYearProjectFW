﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalYearProject.Migrations
{
    /// <inheritdoc />
    public partial class fixProd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_CategoryCode",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "Product",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_CategoryCode",
                table: "Product",
                column: "CategoryCode",
                principalTable: "Category",
                principalColumn: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Category_CategoryCode",
                table: "Product");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryCode",
                table: "Product",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Category_CategoryCode",
                table: "Product",
                column: "CategoryCode",
                principalTable: "Category",
                principalColumn: "Code",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Product.Infrastructure.Data.Migrations
{
    public partial class addtimestamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Timestamp",
                table: "Product",
                rowVersion: true,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Product");
        }
    }
}

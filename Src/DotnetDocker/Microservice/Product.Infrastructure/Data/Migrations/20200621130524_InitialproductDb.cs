using Microsoft.EntityFrameworkCore.Migrations;

namespace Product.Infrastructure.Data.Migrations
{
    public partial class InitialproductDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Category = table.Column<string>(maxLength: 100, nullable: false),
                    Summary = table.Column<string>(maxLength: 520, nullable: false),
                    Description = table.Column<string>(nullable: false),
                    ImageFile = table.Column<string>(maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}

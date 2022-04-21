using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace INK_API.Migrations
{
    public partial class Ver1112AddStockTablesleo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Stocks",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InkOrChemicalID = table.Column<int>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    NameInkOrChemical = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false),
                    ManufacturingDate = table.Column<DateTime>(nullable: false),
                    SupplierName = table.Column<string>(nullable: true),
                    Batch = table.Column<string>(nullable: true),
                    ExpiredTime = table.Column<DateTime>(nullable: false),
                    Unit = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    BuildingName = table.Column<string>(nullable: true),
                    Status = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stocks", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stocks");
        }
    }
}

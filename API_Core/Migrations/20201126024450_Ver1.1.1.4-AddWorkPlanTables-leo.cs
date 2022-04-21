using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace INK_API.Migrations
{
    public partial class Ver1114AddWorkPlanTablesleo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkPlans",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Line = table.Column<string>(nullable: true),
                    PONo = table.Column<string>(nullable: true),
                    ModelName = table.Column<string>(nullable: true),
                    ModelNo = table.Column<string>(nullable: true),
                    ArticleNo = table.Column<string>(nullable: true),
                    Qty = table.Column<string>(nullable: true),
                    Treatment = table.Column<string>(nullable: true),
                    Stitching = table.Column<string>(nullable: true),
                    Stockfitting = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkPlans", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkPlans");
        }
    }
}

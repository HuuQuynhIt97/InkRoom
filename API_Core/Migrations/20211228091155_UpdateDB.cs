using Microsoft.EntityFrameworkCore.Migrations;

namespace INK_API.Migrations
{
    public partial class UpdateDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Percentage",
                table: "PartInkChemicals",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "percentage",
                table: "Inks",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");


            migrationBuilder.AlterColumn<double>(
                name: "Percentage",
                table: "Chemicals",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
        

            migrationBuilder.AlterColumn<int>(
                name: "Percentage",
                table: "PartInkChemicals",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "percentage",
                table: "Inks",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<int>(
                name: "Percentage",
                table: "Chemicals",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}

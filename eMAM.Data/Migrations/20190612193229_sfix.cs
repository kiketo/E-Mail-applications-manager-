using Microsoft.EntityFrameworkCore.Migrations;

namespace eMAM.Data.Migrations
{
    public partial class sfix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CustomerEGN",
                table: "Customers",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Customers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Customers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Customers");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerEGN",
                table: "Customers",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

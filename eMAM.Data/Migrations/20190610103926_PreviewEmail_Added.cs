using Microsoft.EntityFrameworkCore.Migrations;

namespace eMAM.Data.Migrations
{
    public partial class PreviewEmail_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PreviewedById",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "WorkInProcess",
                table: "Emails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Emails_PreviewedById",
                table: "Emails",
                column: "PreviewedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_AspNetUsers_PreviewedById",
                table: "Emails",
                column: "PreviewedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_AspNetUsers_PreviewedById",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_PreviewedById",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "PreviewedById",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "WorkInProcess",
                table: "Emails");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace eMAM.Data.Migrations
{
    public partial class WorkInProcessEmail_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WorkingById",
                table: "Emails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emails_WorkingById",
                table: "Emails",
                column: "WorkingById");

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_AspNetUsers_WorkingById",
                table: "Emails",
                column: "WorkingById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_AspNetUsers_WorkingById",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_WorkingById",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "WorkingById",
                table: "Emails");
        }
    }
}

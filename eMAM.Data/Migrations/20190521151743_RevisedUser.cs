using Microsoft.EntityFrameworkCore.Migrations;

namespace eMAM.Data.Migrations
{
    public partial class RevisedUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_AspNetUsers_UserClosedThisApplicationId",
                table: "Emails");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_AspNetUsers_UserOpenedThisApplicationId",
                table: "Emails");

            migrationBuilder.RenameColumn(
                name: "UserOpenedThisApplicationId",
                table: "Emails",
                newName: "OpenedById");

            migrationBuilder.RenameColumn(
                name: "UserClosedThisApplicationId",
                table: "Emails",
                newName: "ClosedById");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_UserOpenedThisApplicationId",
                table: "Emails",
                newName: "IX_Emails_OpenedById");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_UserClosedThisApplicationId",
                table: "Emails",
                newName: "IX_Emails_ClosedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_AspNetUsers_ClosedById",
                table: "Emails",
                column: "ClosedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_AspNetUsers_OpenedById",
                table: "Emails",
                column: "OpenedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_AspNetUsers_ClosedById",
                table: "Emails");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_AspNetUsers_OpenedById",
                table: "Emails");

            migrationBuilder.RenameColumn(
                name: "OpenedById",
                table: "Emails",
                newName: "UserOpenedThisApplicationId");

            migrationBuilder.RenameColumn(
                name: "ClosedById",
                table: "Emails",
                newName: "UserClosedThisApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_OpenedById",
                table: "Emails",
                newName: "IX_Emails_UserOpenedThisApplicationId");

            migrationBuilder.RenameIndex(
                name: "IX_Emails_ClosedById",
                table: "Emails",
                newName: "IX_Emails_UserClosedThisApplicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_AspNetUsers_UserClosedThisApplicationId",
                table: "Emails",
                column: "UserClosedThisApplicationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Emails_AspNetUsers_UserOpenedThisApplicationId",
                table: "Emails",
                column: "UserOpenedThisApplicationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace eMAM.Data.Migrations
{
    public partial class RevisedEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<List<string>>(
                name: "AttachmentsFileNames",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<List<double>>(
                name: "AttachmentsFilesSizeInMb",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CustomerEGN",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerPhoneNumber",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateReceived",
                table: "Emails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GmailIdNumber",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "InitialRegistrationInSystemOn",
                table: "Emails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "SenderEmail",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SetInCurrentStatusOn",
                table: "Emails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "SetInTerminalStatusOn",
                table: "Emails",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserClosedThisApplicationId",
                table: "Emails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserOpenedThisApplicationId",
                table: "Emails",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Emails_UserClosedThisApplicationId",
                table: "Emails",
                column: "UserClosedThisApplicationId");

            migrationBuilder.CreateIndex(
                name: "IX_Emails_UserOpenedThisApplicationId",
                table: "Emails",
                column: "UserOpenedThisApplicationId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Emails_AspNetUsers_UserClosedThisApplicationId",
                table: "Emails");

            migrationBuilder.DropForeignKey(
                name: "FK_Emails_AspNetUsers_UserOpenedThisApplicationId",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_UserClosedThisApplicationId",
                table: "Emails");

            migrationBuilder.DropIndex(
                name: "IX_Emails_UserOpenedThisApplicationId",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "AttachmentsFileNames",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "AttachmentsFilesSizeInMb",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "CustomerEGN",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "CustomerPhoneNumber",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "DateReceived",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "GmailIdNumber",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "InitialRegistrationInSystemOn",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "SenderEmail",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "SetInCurrentStatusOn",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "SetInTerminalStatusOn",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "UserClosedThisApplicationId",
                table: "Emails");

            migrationBuilder.DropColumn(
                name: "UserOpenedThisApplicationId",
                table: "Emails");
        }
    }
}

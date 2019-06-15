using Microsoft.EntityFrameworkCore.Migrations;

namespace eMAM.Data.Migrations
{
    public partial class auditLogFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Statuses_NewStatusId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Statuses_OldStatusId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_NewStatusId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_OldStatusId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "NewStatusId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "OldStatusId",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AuditLogs",
                newName: "UserName");

            migrationBuilder.AddColumn<string>(
                name: "NewStatus",
                table: "AuditLogs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OldStatus",
                table: "AuditLogs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewStatus",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "OldStatus",
                table: "AuditLogs");

            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "AuditLogs",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "NewStatusId",
                table: "AuditLogs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldStatusId",
                table: "AuditLogs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_NewStatusId",
                table: "AuditLogs",
                column: "NewStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_OldStatusId",
                table: "AuditLogs",
                column: "OldStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Statuses_NewStatusId",
                table: "AuditLogs",
                column: "NewStatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Statuses_OldStatusId",
                table: "AuditLogs",
                column: "OldStatusId",
                principalTable: "Statuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_UserId",
                table: "AuditLogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

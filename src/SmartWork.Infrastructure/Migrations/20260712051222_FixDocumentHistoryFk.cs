using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWork.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDocumentHistoryFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_TaskItems_TaskItemId",
                table: "Files");

            migrationBuilder.DropForeignKey(
                name: "FK_Files_Workspaces_WorkspaceId",
                table: "Files");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.RenameTable(
                name: "Files",
                newName: "FileAssets");

            migrationBuilder.RenameIndex(
                name: "IX_Files_WorkspaceId",
                table: "FileAssets",
                newName: "IX_FileAssets_WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_Files_TaskItemId",
                table: "FileAssets",
                newName: "IX_FileAssets_TaskItemId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModifiedBy",
                table: "DocumentHistories",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FileAssets",
                table: "FileAssets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentHistories_ModifiedBy",
                table: "DocumentHistories",
                column: "ModifiedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_DocumentHistories_AspNetUsers_ModifiedBy",
                table: "DocumentHistories",
                column: "ModifiedBy",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_FileAssets_TaskItems_TaskItemId",
                table: "FileAssets",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FileAssets_Workspaces_WorkspaceId",
                table: "FileAssets",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DocumentHistories_AspNetUsers_ModifiedBy",
                table: "DocumentHistories");

            migrationBuilder.DropForeignKey(
                name: "FK_FileAssets_TaskItems_TaskItemId",
                table: "FileAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_FileAssets_Workspaces_WorkspaceId",
                table: "FileAssets");

            migrationBuilder.DropIndex(
                name: "IX_DocumentHistories_ModifiedBy",
                table: "DocumentHistories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FileAssets",
                table: "FileAssets");

            migrationBuilder.RenameTable(
                name: "FileAssets",
                newName: "Files");

            migrationBuilder.RenameIndex(
                name: "IX_FileAssets_WorkspaceId",
                table: "Files",
                newName: "IX_Files_WorkspaceId");

            migrationBuilder.RenameIndex(
                name: "IX_FileAssets_TaskItemId",
                table: "Files",
                newName: "IX_Files_TaskItemId");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModifiedBy",
                table: "DocumentHistories",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_TaskItems_TaskItemId",
                table: "Files",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Workspaces_WorkspaceId",
                table: "Files",
                column: "WorkspaceId",
                principalTable: "Workspaces",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

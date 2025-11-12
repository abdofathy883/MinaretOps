using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeTaskRelationshipsOptionalForArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTasks_ArchivedTaskId",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_Tasks_TaskId",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_ArchivedTasks_ArchivedTaskId",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_Tasks_TaskItemId",
                table: "TaskHistory");

            migrationBuilder.AlterColumn<int>(
                name: "TaskItemId",
                table: "TaskHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "TaskCompletionResources",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTasks_ArchivedTaskId",
                table: "TaskCompletionResources",
                column: "ArchivedTaskId",
                principalTable: "ArchivedTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_Tasks_TaskId",
                table: "TaskCompletionResources",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_ArchivedTasks_ArchivedTaskId",
                table: "TaskHistory",
                column: "ArchivedTaskId",
                principalTable: "ArchivedTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_Tasks_TaskItemId",
                table: "TaskHistory",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTasks_ArchivedTaskId",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_Tasks_TaskId",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_ArchivedTasks_ArchivedTaskId",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_Tasks_TaskItemId",
                table: "TaskHistory");

            migrationBuilder.AlterColumn<int>(
                name: "TaskItemId",
                table: "TaskHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TaskId",
                table: "TaskCompletionResources",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTasks_ArchivedTaskId",
                table: "TaskCompletionResources",
                column: "ArchivedTaskId",
                principalTable: "ArchivedTasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_Tasks_TaskId",
                table: "TaskCompletionResources",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_ArchivedTasks_ArchivedTaskId",
                table: "TaskHistory",
                column: "ArchivedTaskId",
                principalTable: "ArchivedTasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_Tasks_TaskItemId",
                table: "TaskHistory",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

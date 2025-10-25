using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeparatedTasksAndArchive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_ClientServices_ClientServiceId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Tasks");

            migrationBuilder.AddColumn<int>(
                name: "ArchivedTaskId",
                table: "TaskHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ArchivedTaskId",
                table: "TaskCompletionResources",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArchivedTasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TaskType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClientServiceId = table.Column<int>(type: "int", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Refrence = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaskGroupId = table.Column<int>(type: "int", nullable: false),
                    CompletionNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchivedTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArchivedTasks_AspNetUsers_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ArchivedTasks_ClientServices_ClientServiceId",
                        column: x => x.ClientServiceId,
                        principalTable: "ClientServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArchivedTasks_TaskGroups_TaskGroupId",
                        column: x => x.TaskGroupId,
                        principalTable: "TaskGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_ArchivedTaskId",
                table: "TaskHistory",
                column: "ArchivedTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompletionResources_ArchivedTaskId",
                table: "TaskCompletionResources",
                column: "ArchivedTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedTasks_ClientServiceId",
                table: "ArchivedTasks",
                column: "ClientServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedTasks_EmployeeId",
                table: "ArchivedTasks",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchivedTasks_TaskGroupId",
                table: "ArchivedTasks",
                column: "TaskGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTasks_ArchivedTaskId",
                table: "TaskCompletionResources",
                column: "ArchivedTaskId",
                principalTable: "ArchivedTasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistory_ArchivedTasks_ArchivedTaskId",
                table: "TaskHistory",
                column: "ArchivedTaskId",
                principalTable: "ArchivedTasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_ClientServices_ClientServiceId",
                table: "Tasks",
                column: "ClientServiceId",
                principalTable: "ClientServices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskCompletionResources_ArchivedTasks_ArchivedTaskId",
                table: "TaskCompletionResources");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistory_ArchivedTasks_ArchivedTaskId",
                table: "TaskHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_ClientServices_ClientServiceId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "ArchivedTasks");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistory_ArchivedTaskId",
                table: "TaskHistory");

            migrationBuilder.DropIndex(
                name: "IX_TaskCompletionResources_ArchivedTaskId",
                table: "TaskCompletionResources");

            migrationBuilder.DropColumn(
                name: "ArchivedTaskId",
                table: "TaskHistory");

            migrationBuilder.DropColumn(
                name: "ArchivedTaskId",
                table: "TaskCompletionResources");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Tasks",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_ClientServices_ClientServiceId",
                table: "Tasks",
                column: "ClientServiceId",
                principalTable: "ClientServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

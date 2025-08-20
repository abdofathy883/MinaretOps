using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ClientServiceModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskGroup_ClientServices_ClientServiceId",
                table: "TaskGroup");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskGroup_TaskGroupId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskGroup",
                table: "TaskGroup");

            migrationBuilder.RenameTable(
                name: "TaskGroup",
                newName: "TaskGroups");

            migrationBuilder.RenameIndex(
                name: "IX_TaskGroup_ClientServiceId",
                table: "TaskGroups",
                newName: "IX_TaskGroups_ClientServiceId");

            migrationBuilder.AlterColumn<string>(
                name: "MonthLabel",
                table: "TaskGroups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskGroups",
                table: "TaskGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskGroups_ClientServices_ClientServiceId",
                table: "TaskGroups",
                column: "ClientServiceId",
                principalTable: "ClientServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskGroups_TaskGroupId",
                table: "Tasks",
                column: "TaskGroupId",
                principalTable: "TaskGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskGroups_ClientServices_ClientServiceId",
                table: "TaskGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_TaskGroups_TaskGroupId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TaskGroups",
                table: "TaskGroups");

            migrationBuilder.RenameTable(
                name: "TaskGroups",
                newName: "TaskGroup");

            migrationBuilder.RenameIndex(
                name: "IX_TaskGroups_ClientServiceId",
                table: "TaskGroup",
                newName: "IX_TaskGroup_ClientServiceId");

            migrationBuilder.AlterColumn<string>(
                name: "MonthLabel",
                table: "TaskGroup",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_TaskGroup",
                table: "TaskGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskGroup_ClientServices_ClientServiceId",
                table: "TaskGroup",
                column: "ClientServiceId",
                principalTable: "ClientServices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_TaskGroup_TaskGroupId",
                table: "Tasks",
                column: "TaskGroupId",
                principalTable: "TaskGroup",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

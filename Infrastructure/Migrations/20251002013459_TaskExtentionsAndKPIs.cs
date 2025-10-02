using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TaskExtentionsAndKPIs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletionNotes",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "Date",
                table: "KPIIncedints",
                type: "date",
                nullable: true,
                defaultValueSql: "GETDATE()");

            migrationBuilder.CreateTable(
                name: "TaskCompletionResources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskCompletionResources", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskCompletionResources_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskCompletionResources_TaskId",
                table: "TaskCompletionResources",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskCompletionResources");

            migrationBuilder.DropColumn(
                name: "CompletionNotes",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "KPIIncedints");
        }
    }
}

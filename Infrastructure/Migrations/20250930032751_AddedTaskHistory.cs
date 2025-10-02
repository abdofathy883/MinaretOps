using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedTaskHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaskItemId = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskHistory_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskHistory_Tasks_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_TaskItemId",
                table: "TaskHistory",
                column: "TaskItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistory_UpdatedById",
                table: "TaskHistory",
                column: "UpdatedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskHistory");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlteredDeletionBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_AspNetUsers_EmployeeId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalTaskAssignments_AspNetUsers_UserId",
                table: "InternalTaskAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_EmployeeId",
                table: "Tasks");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Tasks",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "InternalTaskAssignments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_EmployeeId",
                table: "Complaints",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalTaskAssignments_AspNetUsers_UserId",
                table: "InternalTaskAssignments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_EmployeeId",
                table: "Tasks",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Complaints_AspNetUsers_EmployeeId",
                table: "Complaints");

            migrationBuilder.DropForeignKey(
                name: "FK_InternalTaskAssignments_AspNetUsers_UserId",
                table: "InternalTaskAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_EmployeeId",
                table: "Tasks");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeId",
                table: "Tasks",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "InternalTaskAssignments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Complaints_AspNetUsers_EmployeeId",
                table: "Complaints",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InternalTaskAssignments_AspNetUsers_UserId",
                table: "InternalTaskAssignments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_EmployeeId",
                table: "Tasks",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

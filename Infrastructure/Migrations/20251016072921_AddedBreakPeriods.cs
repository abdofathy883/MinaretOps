using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedBreakPeriods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreakPeriod_AttendanceRecords_AttendanceRecordId",
                table: "BreakPeriod");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BreakPeriod",
                table: "BreakPeriod");

            migrationBuilder.RenameTable(
                name: "BreakPeriod",
                newName: "BreakPeriods");

            migrationBuilder.RenameIndex(
                name: "IX_BreakPeriod_AttendanceRecordId",
                table: "BreakPeriods",
                newName: "IX_BreakPeriods_AttendanceRecordId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BreakPeriods",
                table: "BreakPeriods",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BreakPeriods_AttendanceRecords_AttendanceRecordId",
                table: "BreakPeriods",
                column: "AttendanceRecordId",
                principalTable: "AttendanceRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BreakPeriods_AttendanceRecords_AttendanceRecordId",
                table: "BreakPeriods");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BreakPeriods",
                table: "BreakPeriods");

            migrationBuilder.RenameTable(
                name: "BreakPeriods",
                newName: "BreakPeriod");

            migrationBuilder.RenameIndex(
                name: "IX_BreakPeriods_AttendanceRecordId",
                table: "BreakPeriod",
                newName: "IX_BreakPeriod_AttendanceRecordId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BreakPeriod",
                table: "BreakPeriod",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BreakPeriod_AttendanceRecords_AttendanceRecordId",
                table: "BreakPeriod",
                column: "AttendanceRecordId",
                principalTable: "AttendanceRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

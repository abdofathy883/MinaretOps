using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixAttendanceUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_EmployeeId_ClockIn",
                table: "AttendanceRecords");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_EmployeeId_WorkDate",
                table: "AttendanceRecords",
                columns: new[] { "EmployeeId", "WorkDate" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_EmployeeId_WorkDate",
                table: "AttendanceRecords");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_EmployeeId_ClockIn",
                table: "AttendanceRecords",
                columns: new[] { "EmployeeId", "ClockIn" },
                unique: true);
        }
    }
}

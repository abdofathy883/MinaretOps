using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedAttendanceClockOut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CheckInTime",
                table: "AttendanceRecords",
                newName: "ClockIn");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_EmployeeId_CheckInTime",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_EmployeeId_ClockIn");

            migrationBuilder.AddColumn<DateTime>(
                name: "ClockOut",
                table: "AttendanceRecords",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MissingClockOut",
                table: "AttendanceRecords",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClockOut",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "MissingClockOut",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "ClockIn",
                table: "AttendanceRecords",
                newName: "CheckInTime");

            migrationBuilder.RenameIndex(
                name: "IX_AttendanceRecords_EmployeeId_ClockIn",
                table: "AttendanceRecords",
                newName: "IX_AttendanceRecords_EmployeeId_CheckInTime");
        }
    }
}

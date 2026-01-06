using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MinorPayrollFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_AspNetUsers_EmployeeId",
                table: "SalaryPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_SalaryPeriods_SalaryPeriodId",
                table: "SalaryPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPeriods_AspNetUsers_EmployeeId",
                table: "SalaryPeriods");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "SalaryPeriods",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MonthLabel",
                table: "SalaryPeriods",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Deductions",
                table: "SalaryPeriods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SalaryPeriods",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "Bonus",
                table: "SalaryPeriods",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "SalaryPayments",
                type: "nvarchar(3000)",
                maxLength: 3000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SalaryPayments",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPeriods_EmployeeId_Year_Month",
                table: "SalaryPeriods",
                columns: new[] { "EmployeeId", "Year", "Month" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalaryPeriods_MonthLabel",
                table: "SalaryPeriods",
                column: "MonthLabel");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_AspNetUsers_EmployeeId",
                table: "SalaryPayments",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_SalaryPeriods_SalaryPeriodId",
                table: "SalaryPayments",
                column: "SalaryPeriodId",
                principalTable: "SalaryPeriods",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPeriods_AspNetUsers_EmployeeId",
                table: "SalaryPeriods",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_AspNetUsers_EmployeeId",
                table: "SalaryPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPayments_SalaryPeriods_SalaryPeriodId",
                table: "SalaryPayments");

            migrationBuilder.DropForeignKey(
                name: "FK_SalaryPeriods_AspNetUsers_EmployeeId",
                table: "SalaryPeriods");

            migrationBuilder.DropIndex(
                name: "IX_SalaryPeriods_EmployeeId_Year_Month",
                table: "SalaryPeriods");

            migrationBuilder.DropIndex(
                name: "IX_SalaryPeriods_MonthLabel",
                table: "SalaryPeriods");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "SalaryPeriods",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3000)",
                oldMaxLength: 3000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MonthLabel",
                table: "SalaryPeriods",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<decimal>(
                name: "Deductions",
                table: "SalaryPeriods",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SalaryPeriods",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AlterColumn<decimal>(
                name: "Bonus",
                table: "SalaryPeriods",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldDefaultValue: 0m);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "SalaryPayments",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(3000)",
                oldMaxLength: 3000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "SalaryPayments",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETUTCDATE()");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_AspNetUsers_EmployeeId",
                table: "SalaryPayments",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPayments_SalaryPeriods_SalaryPeriodId",
                table: "SalaryPayments",
                column: "SalaryPeriodId",
                principalTable: "SalaryPeriods",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SalaryPeriods_AspNetUsers_EmployeeId",
                table: "SalaryPeriods",
                column: "EmployeeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

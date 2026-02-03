using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmpInvitationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOnBoardingInvitations_AspNetUsers_InvitedByUserId",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeOnBoardingInvitations_Email",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeOnBoardingInvitations_InvitationToken",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentNumber",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NID",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(14)",
                maxLength: 14,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvitedByUserId",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOnBoardingInvitations_AspNetUsers_InvitedByUserId",
                table: "EmployeeOnBoardingInvitations",
                column: "InvitedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeOnBoardingInvitations_AspNetUsers_InvitedByUserId",
                table: "EmployeeOnBoardingInvitations");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "EmployeeOnBoardingInvitations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Role",
                table: "EmployeeOnBoardingInvitations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentNumber",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NID",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(14)",
                oldMaxLength: 14,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastName",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvitedByUserId",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "EmployeeOnBoardingInvitations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOnBoardingInvitations_Email",
                table: "EmployeeOnBoardingInvitations",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeOnBoardingInvitations_InvitationToken",
                table: "EmployeeOnBoardingInvitations",
                column: "InvitationToken",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeOnBoardingInvitations_AspNetUsers_InvitedByUserId",
                table: "EmployeeOnBoardingInvitations",
                column: "InvitedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

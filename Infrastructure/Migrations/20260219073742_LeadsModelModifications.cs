using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LeadsModelModifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactAttempts",
                table: "SalesLeads");

            migrationBuilder.DropColumn(
                name: "DecisionMakerReached",
                table: "SalesLeads");

            migrationBuilder.DropColumn(
                name: "Interested",
                table: "SalesLeads");

            migrationBuilder.DropColumn(
                name: "MeetingAgreed",
                table: "SalesLeads");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "SalesLeads");

            migrationBuilder.RenameColumn(
                name: "MeetingAttend",
                table: "SalesLeads",
                newName: "NeedsExpectation");

            migrationBuilder.RenameColumn(
                name: "FollowUpReason",
                table: "SalesLeads",
                newName: "Occupation");

            migrationBuilder.AlterColumn<string>(
                name: "LeadSource",
                table: "SalesLeads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentLeadStatus",
                table: "SalesLeads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ContactStatus",
                table: "SalesLeads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Budget",
                table: "SalesLeads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "SalesLeads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FreelancePlatform",
                table: "SalesLeads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Responsibility",
                table: "SalesLeads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Timeline",
                table: "SalesLeads",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "LeadNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Note = table.Column<string>(type: "nvarchar(max)", maxLength: 6000, nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadNotes_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LeadNotes_SalesLeads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "SalesLeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadNotes_CreatedById",
                table: "LeadNotes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_LeadNotes_LeadId",
                table: "LeadNotes",
                column: "LeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeadNotes");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "SalesLeads");

            migrationBuilder.DropColumn(
                name: "Country",
                table: "SalesLeads");

            migrationBuilder.DropColumn(
                name: "FreelancePlatform",
                table: "SalesLeads");

            migrationBuilder.DropColumn(
                name: "Responsibility",
                table: "SalesLeads");

            migrationBuilder.DropColumn(
                name: "Timeline",
                table: "SalesLeads");

            migrationBuilder.RenameColumn(
                name: "Occupation",
                table: "SalesLeads",
                newName: "FollowUpReason");

            migrationBuilder.RenameColumn(
                name: "NeedsExpectation",
                table: "SalesLeads",
                newName: "MeetingAttend");

            migrationBuilder.AlterColumn<string>(
                name: "LeadSource",
                table: "SalesLeads",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "CurrentLeadStatus",
                table: "SalesLeads",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ContactStatus",
                table: "SalesLeads",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<int>(
                name: "ContactAttempts",
                table: "SalesLeads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DecisionMakerReached",
                table: "SalesLeads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Interested",
                table: "SalesLeads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MeetingAgreed",
                table: "SalesLeads",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "SalesLeads",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}

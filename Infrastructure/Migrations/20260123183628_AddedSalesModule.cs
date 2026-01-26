using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedSalesModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalesLeads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BusinessName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    WhatsAppNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ContactAttempts = table.Column<int>(type: "int", nullable: false),
                    ContactStatus = table.Column<int>(type: "int", nullable: false),
                    LeadSource = table.Column<int>(type: "int", nullable: false),
                    DecisionMakerReached = table.Column<bool>(type: "bit", nullable: false),
                    Interested = table.Column<bool>(type: "bit", nullable: false),
                    InterestLevel = table.Column<int>(type: "int", nullable: false),
                    MeetingAgreed = table.Column<bool>(type: "bit", nullable: false),
                    MeetingDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MeetingAttend = table.Column<int>(type: "int", nullable: false),
                    QuotationSent = table.Column<bool>(type: "bit", nullable: false),
                    FollowUpTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FollowUpReason = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    SalesRepId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesLeads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesLeads_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesLeads_AspNetUsers_SalesRepId",
                        column: x => x.SalesRepId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeadServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    LeadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LeadServices_SalesLeads_LeadId",
                        column: x => x.LeadId,
                        principalTable: "SalesLeads",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeadServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeadServices_LeadId",
                table: "LeadServices",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_LeadServices_ServiceId",
                table: "LeadServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesLeads_CreatedById",
                table: "SalesLeads",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SalesLeads_SalesRepId",
                table: "SalesLeads",
                column: "SalesRepId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LeadServices");

            migrationBuilder.DropTable(
                name: "SalesLeads");
        }
    }
}

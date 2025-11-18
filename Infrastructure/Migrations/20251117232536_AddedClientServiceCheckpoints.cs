using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedClientServiceCheckpoints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Bio",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JobTitle",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "AspNetUsers");

            migrationBuilder.CreateTable(
                name: "ServiceCheckpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceCheckpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceCheckpoints_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientServiceCheckpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientServiceId = table.Column<int>(type: "int", nullable: false),
                    ServiceCheckpointId = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedByEmployeeId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientServiceCheckpoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientServiceCheckpoints_AspNetUsers_CompletedByEmployeeId",
                        column: x => x.CompletedByEmployeeId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ClientServiceCheckpoints_ClientServices_ClientServiceId",
                        column: x => x.ClientServiceId,
                        principalTable: "ClientServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientServiceCheckpoints_ServiceCheckpoints_ServiceCheckpointId",
                        column: x => x.ServiceCheckpointId,
                        principalTable: "ServiceCheckpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientServiceCheckpoints_ClientServiceId",
                table: "ClientServiceCheckpoints",
                column: "ClientServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientServiceCheckpoints_ClientServiceId_ServiceCheckpointId",
                table: "ClientServiceCheckpoints",
                columns: new[] { "ClientServiceId", "ServiceCheckpointId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientServiceCheckpoints_CompletedByEmployeeId",
                table: "ClientServiceCheckpoints",
                column: "CompletedByEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientServiceCheckpoints_ServiceCheckpointId",
                table: "ClientServiceCheckpoints",
                column: "ServiceCheckpointId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCheckpoints_ServiceId",
                table: "ServiceCheckpoints",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceCheckpoints_ServiceId_Order",
                table: "ServiceCheckpoints",
                columns: new[] { "ServiceId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientServiceCheckpoints");

            migrationBuilder.DropTable(
                name: "ServiceCheckpoints");

            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JobTitle",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

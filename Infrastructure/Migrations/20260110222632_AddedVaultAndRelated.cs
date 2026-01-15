using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedVaultAndRelated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vaults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaultType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaults", x => x.Id);
                    table.CheckConstraint("CK_Vault_UnifiedVaultConstraint", "([VaultType] = 'Branch' AND [BranchId] IS NOT NULL) OR ([VaultType] = 'Unified' AND [BranchId] IS NULL)");
                    table.ForeignKey(
                        name: "FK_Vaults_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Vaults_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VaultTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VaultId = table.Column<int>(type: "int", nullable: false),
                    TransactionType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaultTransactions", x => x.Id);
                    table.CheckConstraint("CK_VaultTransaction_AmountPositive", "[Amount] > 0");
                    table.ForeignKey(
                        name: "FK_VaultTransactions_AspNetUsers_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaultTransactions_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VaultTransactions_Vaults_VaultId",
                        column: x => x.VaultId,
                        principalTable: "Vaults",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Code",
                table: "Branches",
                column: "Code",
                unique: true,
                filter: "[Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_Name",
                table: "Branches",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Vaults_BranchId",
                table: "Vaults",
                column: "BranchId",
                unique: true,
                filter: "[BranchId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Vaults_CurrencyId",
                table: "Vaults",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Vaults_VaultType",
                table: "Vaults",
                column: "VaultType");

            migrationBuilder.CreateIndex(
                name: "IX_VaultTransactions_CreatedById",
                table: "VaultTransactions",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_VaultTransactions_CurrencyId",
                table: "VaultTransactions",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultTransactions_ReferenceId",
                table: "VaultTransactions",
                column: "ReferenceId",
                filter: "[ReferenceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VaultTransactions_ReferenceType",
                table: "VaultTransactions",
                column: "ReferenceType");

            migrationBuilder.CreateIndex(
                name: "IX_VaultTransactions_TransactionDate",
                table: "VaultTransactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_VaultTransactions_TransactionType",
                table: "VaultTransactions",
                column: "TransactionType");

            migrationBuilder.CreateIndex(
                name: "IX_VaultTransactions_VaultId",
                table: "VaultTransactions",
                column: "VaultId");

            migrationBuilder.CreateIndex(
                name: "IX_VaultTransactions_VaultId_TransactionDate",
                table: "VaultTransactions",
                columns: new[] { "VaultId", "TransactionDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VaultTransactions");

            migrationBuilder.DropTable(
                name: "Vaults");

            migrationBuilder.DropTable(
                name: "Branches");
        }
    }
}

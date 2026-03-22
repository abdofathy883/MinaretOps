using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtendPortfolioToBeMultilingual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioItems");

            migrationBuilder.DropTable(
                name: "PortfolioCategories");

            migrationBuilder.CreateTable(
                name: "PortfolioCategory",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioCategoryTranslation",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioCategoryTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioCategoryTranslation_PortfolioCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Content",
                        principalTable: "PortfolioCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioItem",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ImageLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    PortfolioCategoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioItem_PortfolioCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "Content",
                        principalTable: "PortfolioCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PortfolioItem_PortfolioCategory_PortfolioCategoryId",
                        column: x => x.PortfolioCategoryId,
                        principalSchema: "Content",
                        principalTable: "PortfolioCategory",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "PortfolioTranslation",
                schema: "Content",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ImageAltText = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    LanguageCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PortfolioItemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioTranslation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioTranslation_PortfolioItem_PortfolioItemId",
                        column: x => x.PortfolioItemId,
                        principalSchema: "Content",
                        principalTable: "PortfolioItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioCategoryTranslation_CategoryId",
                schema: "Content",
                table: "PortfolioCategoryTranslation",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItem_CategoryId",
                schema: "Content",
                table: "PortfolioItem",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItem_PortfolioCategoryId",
                schema: "Content",
                table: "PortfolioItem",
                column: "PortfolioCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioTranslation_PortfolioItemId",
                schema: "Content",
                table: "PortfolioTranslation",
                column: "PortfolioItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioCategoryTranslation",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "PortfolioTranslation",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "PortfolioItem",
                schema: "Content");

            migrationBuilder.DropTable(
                name: "PortfolioCategory",
                schema: "Content");

            migrationBuilder.CreateTable(
                name: "PortfolioCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: true),
                    ImageAltText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PortfolioCategoryId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioItems_PortfolioCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "PortfolioCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PortfolioItems_PortfolioCategories_PortfolioCategoryId",
                        column: x => x.PortfolioCategoryId,
                        principalTable: "PortfolioCategories",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_CategoryId",
                table: "PortfolioItems",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_PortfolioCategoryId",
                table: "PortfolioItems",
                column: "PortfolioCategoryId");
        }
    }
}

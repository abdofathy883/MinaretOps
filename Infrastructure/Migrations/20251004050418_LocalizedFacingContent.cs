using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LocalizedFacingContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContentLanguageId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContentLanguageId",
                table: "ProjectCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "ProjectCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContentLanguageId",
                table: "BlogPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "BlogPosts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ContentLanguageId",
                table: "BlogCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "BlogCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentLanguageId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ContentLanguageId",
                table: "ProjectCategories");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "ProjectCategories");

            migrationBuilder.DropColumn(
                name: "ContentLanguageId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "ContentLanguageId",
                table: "BlogCategories");

            migrationBuilder.DropColumn(
                name: "Language",
                table: "BlogCategories");
        }
    }
}

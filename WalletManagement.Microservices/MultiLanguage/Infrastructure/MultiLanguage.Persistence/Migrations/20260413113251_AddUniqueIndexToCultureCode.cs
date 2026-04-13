using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MultiLanguage.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToCultureCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Languages_CultureCode",
                table: "Languages",
                column: "CultureCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Languages_CultureCode",
                table: "Languages");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSVProcessor.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Films_Budget",
                table: "Films",
                column: "Budget");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Films_Budget",
                table: "Films");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSVProcessor.Migrations
{
    /// <inheritdoc />
    public partial class BudgetNotNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Budget",
                table: "Films",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Budget",
                table: "Films",
                nullable: true,
                oldClrType: typeof(long),
                oldNullable: false
            );
        }
    }
}

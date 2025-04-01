using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CSVProcessor.Migrations
{
    /// <inheritdoc />
    public partial class BudgetToLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "BudgetLong",
                table: "Films",
                nullable: true
            );
            migrationBuilder.Sql(@"
        UPDATE ""Films""
        SET ""BudgetLong"" = CAST(REPLACE(""Budget"", '$', '')::decimal AS bigint)
        WHERE ""Budget"" ~ '^\$?[0-9]+(\.[0-9]+)?$';
    ");

            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Films");

            migrationBuilder.RenameColumn(
                name: "BudgetLong",
                table: "Films",
                newName: "Budget");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BudgetString",
                table: "Films",
                type: "text",
                nullable: true);
            
            migrationBuilder.Sql(@"
        UPDATE ""Films""
        SET ""BudgetString"" = CAST(""Budget"" AS text)
    ");
            
            migrationBuilder.DropColumn(
                name: "Budget",
                table: "Films");
            
            migrationBuilder.RenameColumn(
                name: "BudgetString",
                table: "Films",
                newName: "Budget");
        }
    }
}

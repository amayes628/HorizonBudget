using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace HorizonBudget.Data.Migrations;
/// <inheritdoc />   
public partial class CategoryId2LedgerId : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(name: "CategoryId", table: "Incomes", newName: "LedgerId");
        migrationBuilder.RenameColumn(name: "CategoryId", table: "Expenses", newName: "LedgerId");
        migrationBuilder.RenameColumn(name: "CategoryId", table: "Accounts", newName: "LedgerId");
    }
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(name: "LedgerId", table: "Incomes", newName: "CategoryId"); 
        migrationBuilder.RenameColumn(name: "LedgerId", table: "Expenses", newName: "CategoryId"); 
        migrationBuilder.RenameColumn(name: "LedgerId", table: "Accounts", newName: "CategoryId");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HorizonBudget.Data;
public class HorizonBudgetContextFactory : IDesignTimeDbContextFactory<HorizonBudgetContext>
{
    public HorizonBudgetContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<HorizonBudgetContext>();

        var dbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "hbdata_designtime.db");

        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new HorizonBudgetContext(optionsBuilder.Options);
    }
}

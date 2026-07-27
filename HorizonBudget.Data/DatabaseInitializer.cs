
using Microsoft.EntityFrameworkCore;

namespace HorizonBudget.Data;
public sealed class DatabaseInitializer(IDbContextFactory<HorizonBudgetContext> factory)
{
    private readonly IDbContextFactory<HorizonBudgetContext> _factory = factory;

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        await using var ctx = _factory.CreateDbContext();

        // Apply migrations
        await ctx.Database.MigrateAsync(ct);

        // Optional: seed data here
        // if (!ctx.Accounts.Any())
        // {
        //     ctx.Accounts.Add(new Account { ... });
        //     await ctx.SaveChangesAsync(ct);
        // }
    }
    public void Initialize()
    {
        InitializeAsync(CancellationToken.None)
            .GetAwaiter()
            .GetResult();
    }
}

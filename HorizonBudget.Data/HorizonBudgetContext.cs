using HorizonBudget.Data.Records;
using Microsoft.EntityFrameworkCore;

namespace HorizonBudget.Data;

public partial class HorizonBudgetContext(DbContextOptions<HorizonBudgetContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<Recurrence> Recurrences => Set<Recurrence>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Ignore<LedgerEntry>();
    }
}

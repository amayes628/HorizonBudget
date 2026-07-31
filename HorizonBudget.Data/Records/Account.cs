using HorizonBudget.Data.Types;

namespace HorizonBudget.Data.Records;
public sealed partial record Account
{
    // Mapped scalar properties
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string AccountNumberSuffix { get; init; } = string.Empty;
    public decimal OpeningBalance { get; init; }
    public decimal CurrentBalance { get; init; }
    public decimal ClosingBalance { get; init; }
    public uint LedgerId { get; init; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
    public EntityStatus Status { get; init; } = EntityStatus.Active;
    public DateOnly? ClosedOn { get; init; }

    // Non‑mapped domain properties
    public LedgerEntry LedgerEntry { get; init; } = LedgerEntry.Empty;

    // Domain logic preserved
    public DateOnly OpenedOn => DateOnly.FromDateTime(CreatedOn);

    public Account()
    {
        
    }
    public Account(
        string name,
        string accountNumberSuffix,
        decimal openingBalance,
        uint categoryId)
    {
        Id = Guid.NewGuid();
        Name = name;
        AccountNumberSuffix = accountNumberSuffix;
        OpeningBalance = openingBalance;
        CurrentBalance = openingBalance; 
        ClosingBalance = openingBalance;
        LedgerId = categoryId;
        Status = EntityStatus.Active;
        ClosedOn = null;
        CreatedOn = DateTime.UtcNow;
        ModifiedOn = DateTime.UtcNow;
    }

    public Account(
        Guid id,
        string name, 
        string accountNumberSuffix,
        decimal openingBalance,
        decimal currentBalance,
        decimal closingBalance,
        uint categoryId,
        EntityStatus accountStatus,
        DateOnly? closedOn,
        DateTime createdOn,
        DateTime modifiedOn)
    {
        Id = id;
        Name = name;
        AccountNumberSuffix = accountNumberSuffix;
        OpeningBalance = openingBalance;
        CurrentBalance = currentBalance;
        ClosingBalance = closingBalance;
        LedgerId = categoryId;
        Status = accountStatus;
        ClosedOn = closedOn;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public bool IsClosed =>
        Status == EntityStatus.Closed &&
        ClosedOn.HasValue &&
        ClosedOn.Value <= DateOnly.FromDateTime(DateTime.Today) &&
        CurrentBalance == 0m;

    // Immutable empty factory
    public static Account Empty => new()
    {
        Id = Guid.Empty,
        Name = "Unassigned Account",
        AccountNumberSuffix = string.Empty,
        OpeningBalance = 0m,
        CurrentBalance = 0m,
        ClosingBalance = 0m,
        LedgerId = 0u,
        CreatedOn = DateTime.UtcNow,
        ModifiedOn = DateTime.UtcNow,
        Status = EntityStatus.Active,
        ClosedOn = null,
        LedgerEntry = LedgerEntry.Empty
    };
}

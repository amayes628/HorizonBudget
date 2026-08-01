using HorizonBudget.Data.Types;
using Microsoft.EntityFrameworkCore;

namespace HorizonBudget.Data.Records;

public sealed partial record Account
{
    // Mapped scalar properties
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string AccountNumberSuffix { get; init; } = string.Empty;
    public decimal OpeningBalance { get; init; }
    public decimal CurrentBalance { get; internal set; }
    public decimal ClosingBalance { get; private set; }
    public uint LedgerId { get; init; }
    internal DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedOn { get; private set; } = DateTime.UtcNow;
    public LedgerStatus Status { get; private set; } = LedgerStatus.Active;
    public DateOnly? ClosedOn { get; private set; }

    // Non‑mapped domain properties
    public LedgerEntry LedgerEntry { get; init; } = LedgerEntry.Empty;

    // Domain logic preserved
    internal DateOnly OpenedOn => DateOnly.FromDateTime(CreatedOn);

    public Account()
    {

    }
    public Account(
        string name,
        string accountNumberSuffix,
        decimal openingBalance,
        uint ledgerId)
    {
        Id = Guid.NewGuid();
        Name = name;
        AccountNumberSuffix = accountNumberSuffix;
        OpeningBalance = openingBalance;
        CurrentBalance = openingBalance;
        ClosingBalance = openingBalance;
        LedgerId = ledgerId;
        Status = LedgerStatus.Active;
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
        uint ledgerId,
        LedgerStatus accountStatus,
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
        LedgerId = ledgerId;
        Status = accountStatus;
        ClosedOn = closedOn;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public bool IsClosed =>
        Status == LedgerStatus.Closed &&
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
        Status = LedgerStatus.Active,
        ClosedOn = null,
        LedgerEntry = LedgerEntry.Empty
    };

    public static Account Touch(Account account)
    {
        return account with
        {
            ModifiedOn = DateTime.UtcNow
        };
    }
    public Account Close()
    {
        if (CurrentBalance != 0)
            throw new InvalidOperationException("Cannot close account with non-zero balance.");

        return this with
        {
            Status = LedgerStatus.Closed,
            ClosingBalance = 0,
            ClosedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            ModifiedOn = DateTime.UtcNow
        };
    }


}

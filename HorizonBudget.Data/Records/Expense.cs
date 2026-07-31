using System;
using HorizonBudget.Data.Records;

namespace HorizonBudget.Data.Records;

public sealed partial record Expense
{
    public Expense(Guid id,
                   string name,
                   decimal amount,
                   decimal variability,
                   bool isEssential,
                   DateOnly? nextDueDate,
                   uint ledgerId,
                   Recurrence recurrence,
                   DateTime createdOn,
                   DateTime modifiedOn,
                   LedgerEntry ledgerEntry)
    {
        Id = id;
        Name = name;
        Amount = amount;
        Variability = variability;
        IsEssential = isEssential;
        NextDueDate = nextDueDate;
        LedgerId = ledgerId;
        Recurrence = recurrence;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
        LedgerEntry = ledgerEntry;
    }
    public Expense()
    {
        
    }
    // Mapped scalar properties
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal Variability { get; init; }
    public bool IsEssential { get; init; }
    public DateOnly? NextDueDate { get; init; }
    public uint LedgerId { get; init; }
    public Recurrence Recurrence { get; init; } = Recurrence.None;
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedOn { get; init; } = DateTime.UtcNow;

    // Non‑mapped domain properties (EF ignores these)
    public LedgerEntry LedgerEntry { get; init; } = LedgerEntry.Empty;


    public static Expense Empty => new()
    {
        Id = Guid.Empty,
        Name = string.Empty,
        Amount = 0m,
        Variability = 0m,
        IsEssential = false,
        NextDueDate = null,
        LedgerId = 0u,
        Recurrence = Recurrence.None,
        CreatedOn = DateTime.UtcNow,
        ModifiedOn = DateTime.UtcNow,
        LedgerEntry = LedgerEntry.Empty
    };
}

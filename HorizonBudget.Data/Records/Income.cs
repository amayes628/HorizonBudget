using System;
using HorizonBudget.Data.Records;
namespace HorizonBudget.Data.Records;

public sealed partial record Income
{
    public Income(Guid id,
                  string name,
                  decimal amount,
                  decimal variability,
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
        LedgerId = ledgerId;
        Recurrence = recurrence;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
        LedgerEntry = ledgerEntry;
    }

    // Mapped scalar properties
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public decimal Variability { get; init; }
    public uint LedgerId { get; init; }
    public Recurrence Recurrence { get; init; } = Recurrence.None;
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedOn { get; init; } = DateTime.UtcNow;

    // Non‑mapped domain properties
    public LedgerEntry LedgerEntry { get; init; } = LedgerEntry.Empty;
    public Income()
    {
        
    }
    // Immutable empty factory
    public static Income Empty => new()
    {
        Id = Guid.Empty,
        Name = "Unassigned Income Source",
        Amount = 0m,
        Variability = 0m,
        LedgerId = 0u,
        Recurrence = Recurrence.None,
        CreatedOn = DateTime.UtcNow,
        ModifiedOn = DateTime.UtcNow,
        LedgerEntry = LedgerEntry.Empty
    };
}

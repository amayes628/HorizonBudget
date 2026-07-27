using HorizonBudget.Data.Types;

namespace HorizonBudget.Data.Records;

/// <summary>
/// Immutable ledger entry line item.
/// </summary>
public sealed partial record Transaction
{
    // Mapped scalar properties
    public Guid Id { get; init; }
    public Guid SourceAccountId { get; set; }
    public Guid TargetAccountId { get; set; }
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
    public TransactionType Type { get; init; }
    public string Description { get; init; } = string.Empty;

    // REQUIRED for EF Core materialization
    public Transaction() { }

    public Transaction(Guid id,
                       Guid sourceAccountId,
                       Guid targetAccountId,
                       DateTime date,
                       decimal amount,
                       TransactionType type,
                       string description = "")
    {
        Id = id;
        SourceAccountId = sourceAccountId;
        TargetAccountId = targetAccountId;
        Date = date;
        Amount = amount;
        Type = type;
        Description = description;

    }

    // Immutable empty factory
    public static Transaction Empty => new()
    {
        Id = Guid.Empty,
        SourceAccountId = Guid.Empty,
        TargetAccountId = Guid.Empty,
        Date = DateTime.MinValue,
        Amount = 0m,
        Description = string.Empty,
        Type = TransactionType.Undefined
    };
}

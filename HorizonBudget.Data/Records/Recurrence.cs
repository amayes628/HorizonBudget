using HorizonBudget.Data.Records;
using HorizonBudget.Data.Types;

namespace HorizonBudget.Data.Records;

/// <summary>
/// A completely independent, immutable domain record representing a milestone frequency cadence rule.
/// </summary>
public sealed partial record Recurrence
{
    // Mapped scalar properties
    public Guid Id { get; init; }
    public RecurrenceUnit Unit { get; init; }
    public int Interval { get; init; }
    public DateOnly StartDate { get; init; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;

    public Recurrence() { }
    // Domain-friendly constructor overload (not used by EF)
    public Recurrence(DateOnly startDate, RecurrenceUnit unit = RecurrenceUnit.Once, int interval = 1)
    {
        Id = Guid.NewGuid();
        Unit = unit;
        Interval = interval;
        StartDate = startDate;
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Static structural fallback representing a single, non-repeating event window.
    /// </summary>
    public static Recurrence Once => new()
    {
        Id = Guid.Empty,
        Unit = RecurrenceUnit.Once,
        Interval = 1,
        StartDate = DateOnly.Parse("2026-07-01"),
        CreatedOn = DateTime.Parse("2026-07-01")
    };

    public static Recurrence None => new()
    {
        Id = Guid.Empty,
        Unit = RecurrenceUnit.None,
        Interval = 0,
        StartDate = DateOnly.FromDateTime(DateTime.Today),
        CreatedOn = DateTime.UtcNow
    };
}

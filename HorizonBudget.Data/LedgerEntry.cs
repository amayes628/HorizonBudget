using HorizonBudget.Data.Types;

namespace HorizonBudget.Data;

public sealed partial record LedgerEntry(
    uint Code,
    string Key,
    LedgerType Type,
    string LocalizedName)
{
    public static LedgerEntry Empty => new(0u, string.Empty, LedgerType.Undefined, "en");
}

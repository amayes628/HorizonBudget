using System.Collections.ObjectModel;
using HorizonBudget.Data;

namespace HorizonBudget.Services;

public interface ILedgerKeyLookupFactory
{
    ObservableCollection<LedgerEntry> AllLedgerKeys { get; }
    IReadOnlyList<LedgerEntry> MasterLedgerKeys { get; }

    LedgerEntry Get(uint code);
    Task InitializeAsync();

}

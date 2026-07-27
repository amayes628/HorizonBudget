
using System.Collections.ObjectModel;
using HorizonBudget.Data;

namespace  HorizonBudget.Services;

public interface ICategoryLookupFactory
{
    ObservableCollection<LedgerEntry> AllCategories { get; }
    IReadOnlyList<LedgerEntry> MasterCat { get; set; }

    LedgerEntry Get(uint code);
    Task InitializeAsync();
}

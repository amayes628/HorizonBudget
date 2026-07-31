using CommunityToolkit.Mvvm.ComponentModel;
using HorizonBudget.Data.Records;
using HorizonBudget.Services;

namespace HorizonBudget.ViewModels;

public class AccountItemViewModel : ObservableObject
{
    private readonly ICultureService _culture;

    public AccountItemViewModel(Account account, ICultureService culture)
    {
        Account = account;
        _culture = culture;

        LocalizedName = _culture.TranslateLedgerKeyPath(account.LedgerId);
    }

    public Account Account { get; }

    public string LocalizedName { get; }

    public string AccountNumberSuffix => Account.AccountNumberSuffix;

    public decimal CurrentBalance => Account.CurrentBalance;
}

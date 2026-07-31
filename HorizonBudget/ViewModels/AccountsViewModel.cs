using System.Collections.ObjectModel;
using System.Security.Principal;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HorizonBudget.Data;
using HorizonBudget.Data.Records;
using HorizonBudget.Services;
using HorizonBudget.Views;

namespace HorizonBudget.ViewModels;

public partial class AccountsViewModel(
    INavigator navigator,
    IRecordRepository<Account> accountRepository,
    ICultureService culture,
    ILedgerKeyLookupFactory lookup) : ObservableObject
{
    private readonly INavigator _navigator = navigator;
    private readonly IRecordRepository<Account> _accountRepository = accountRepository;
    private readonly ICultureService _culture = culture;
    private readonly ILedgerKeyLookupFactory _lookup = lookup;

    public ObservableCollection<AccountItemViewModel> Accounts { get; } = [];

    public async Task LoadAccountsAsync()
    {
        var accounts = await _accountRepository.GetAllAsync();
        Accounts.Clear();
        foreach (var account in accounts)
            Accounts.Add(new AccountItemViewModel(account, _culture));
    }

    // ----------------------------------------------------
    // Commands used by AccountsPage
    // ----------------------------------------------------

    [RelayCommand]
    private async Task AddAccount()
    {
        // This targets the nearest parent frame (your HomePage NavigationView content frame)
        await _navigator.NavigateDataAsync(this, data: Account.Empty, "ManageAccount");
    }

    [RelayCommand]
    private async Task ManageAccount(Account account)
    {
        if (account is null)
            return;

        // Navigate to ManageAccountPage with the selected account
        await _navigator.NavigateViewAsync<ManageAccountPage>(this, data: account);
    }
}

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HorizonBudget.Data;
using HorizonBudget.Data.Records;
using HorizonBudget.Data.Types;
using HorizonBudget.Services;
using HorizonBudget.Validation;
using HorizonBudget.Views;
using HorizonBudget.Views.Pages;

namespace HorizonBudget.ViewModels;

public partial class ManageAccountViewModel : ObservableValidator
{
    #region Dependencies

    private readonly INavigator _navigator;
    private readonly IRecordRepository<Account> _accounts;
    private readonly ILedgerKeyLookupFactory _ledgerLookup;
    private readonly ICultureService _culture;

    #endregion

    #region Mode Awareness

    [ObservableProperty]
    public partial bool IsNew { get; set; }

    [ObservableProperty]
    public partial bool IsEditing { get; set; }

    [ObservableProperty]
    public partial bool IsReadOnly { get; set; }

    [ObservableProperty]
    public partial bool IsClosing { get; set; }

    [ObservableProperty]
    public partial bool HasUnsavedChanges { get; set; }

    #endregion

    #region Account Model

    [ObservableProperty]
    public partial Account Account { get; set; } = new();
    [ObservableProperty]
    public partial uint SelectedLedgerCode { get; set; }
    [ObservableProperty] public partial EntityStatus Status { get; set; }
    #endregion

    #region Ledger Suggestions

    // The list shown in the AutoSuggestionBox (LedgerEntry.Code)
    public ObservableCollection<string> LedgerSuggestions { get; } = [];

    #endregion

    #region Validation Fields

    [ObservableProperty]
    public partial string? NameError { get; set; }

    [ObservableProperty]
    public partial string? LedgerError { get; set; }

    [ObservableProperty]
    public partial string? OpeningBalanceError { get; set; }

    #endregion

    #region UI TeachingTip

    [ObservableProperty]
    public partial bool ShowTeachingTip { get; set; }

    [ObservableProperty]
    public partial string TeachingTipMessage { get; set; } = string.Empty;

    #endregion

    #region Constructor

    public ManageAccountViewModel(
        INavigator navigator,
        IRecordRepository<Account> accounts,
        ILedgerKeyLookupFactory ledgerLookup,
        ICultureService culture)
    {
        _navigator = navigator;
        _accounts = accounts;
        _ledgerLookup = ledgerLookup;
        _culture = culture;

        LoadLedgerSuggestions();
    }

    #endregion

    #region Initialization

    public void Initialize(Account? account)
    {
        if (account is null)
        {
            // NEW ACCOUNT MODE
            IsNew = true;
            IsEditing = true;
            IsReadOnly = false;

            Account = new Account
            {
                Status = EntityStatus.Active,
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = DateTime.UtcNow,
                OpeningBalance = 0,
                CurrentBalance = 0,
                ClosingBalance = 0
            };

            SelectedLedgerCode = 0;
        }
        else
        {
            // EXISTING ACCOUNT MODE
            IsNew = false;
            IsEditing = false;
            IsReadOnly = true;

            Account = account;

            // Map LedgerId → LedgerEntry.Code
            var entry = _ledgerLookup.Get(Account.LedgerEntry.Code);

            SelectedLedgerCode = entry.Code;
        }
    }

    private void LoadLedgerSuggestions()
    {
        LedgerSuggestions.Clear();
        foreach (var key in _ledgerLookup.AllLedgerKeys)
            LedgerSuggestions.Add(key.Key);
    }

    #endregion

    #region Validation Logic (Data Annotations + Custom Rules)

    private bool Validate()
    {
        NameError = null;
        LedgerError = null;
        OpeningBalanceError = null;

        bool ok = true;

        // Data annotation validation
        ValidateAllProperties();

        // Name
        if (string.IsNullOrWhiteSpace(Account.Name) ||
            Account.Name.Length < 3 ||
            Account.Name.Length > 50)
        {
            NameError = ValidationMessages.NameLength;
            ok = false;
        }

        // LedgerKey
        if (string.IsNullOrWhiteSpace(Account.LedgerEntry.Key))
        {
            LedgerError = ValidationMessages.LedgerRequired;
            ok = false;
        }

        // OpeningBalance (only for new accounts)
        if (IsNew && Account.OpeningBalance < 0)
        {
            OpeningBalanceError = ValidationMessages.OpeningBalanceInvalid;
            ok = false;
        }

        return ok;
    }

    #endregion

    #region Commands - Add / Edit / Close / Delete / Cancel
    // The framework automatically injects the passed Account data here
    public void Inject(Account data)
    {
        Account = data;
    }

    [RelayCommand]
    private async Task SaveNewAsync()
    {
        if (!Validate())
        {
            ShowTip(ValidationMessages.CorrectHighlightedFields);
            return;
        }
        var newAccount = new Account
        {
            CurrentBalance = Account.OpeningBalance,
            ClosingBalance = Account.OpeningBalance,
            CreatedOn = DateTime.UtcNow,
            ModifiedOn = Account.CreatedOn,
            Status = EntityStatus.Active,
        };
        await _accounts.AddAsync(newAccount);

        await _navigator.NavigateViewAsync<AccountsPage>(this);
    }

    [RelayCommand]
    private void BeginEdit()
    {
        IsEditing = true;
        IsReadOnly = false;
    }

    [RelayCommand]
    private async Task SaveEditAsync()
    {
        if (!Validate())
        {
            ShowTip(ValidationMessages.CorrectHighlightedFields);
            return;
        }

        Account.ModifiedOn = DateTime.UtcNow;

        await _accounts.UpdateAsync(Account);

        await _navigator.NavigateViewAsync<AccountsPage>(this);
    }

    [RelayCommand]
    private void BeginClose()
    {
        if (Account.CurrentBalance != 0)
        {
            ShowTip(ValidationMessages.CannotCloseBalanceNotZero);
            return;
        }

        IsClosing = true;
        IsEditing = true;
        IsReadOnly = false;
    }

    [RelayCommand]
    private async Task ConfirmCloseAsync()
    {
        Account = Account with
        {
            Status = EntityStatus.Closed,
            ClosingBalance = 0,
            ClosedOn = DateOnly.FromDateTime(DateTime.UtcNow),
            ModifiedOn = DateTime.UtcNow
        };
        Account.ModifiedOn = DateTime.UtcNow;

        await _accounts.UpdateAsync(Account);

        await _navigator.NavigateViewAsync<HomePage>(this);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        await _accounts.DeleteAsync(Account.Id);
        await _navigator.NavigateViewAsync<HomePage>(this);
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        if (HasUnsavedChanges)
        {
            ShowTip(ValidationMessages.UnsavedChanges);
            return;
        }

        await _navigator.NavigateViewAsync<AccountsPage>(this);
    }

    #endregion

    #region TeachingTip Helper

    private void ShowTip(string message)
    {
        TeachingTipMessage = message;
        ShowTeachingTip = true;
    }

    #endregion
}

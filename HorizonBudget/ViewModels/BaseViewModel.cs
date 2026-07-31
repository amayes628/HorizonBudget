using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using HorizonBudget.Data;
using HorizonBudget.Services;

namespace HorizonBudget.ViewModels;

// 1. Inherit from ObservableValidator to unlock standard data annotations
public abstract partial class BaseViewModel : ObservableValidator
{
    //protected static readonly //Logger //Logger = LogManager.GetCurrentClassLogger();
    protected readonly ICultureService Culture;
    protected readonly ILedgerKeyLookupFactory LedgerKeyLookup;
    #region Constant Strings
    // ---------------------------
    // Regular Expressions
    // ---------------------------
    public const string DigitsOnlyRegex = @"^\d*(\.\d+)?$";
    public const string AlphaNumericRegex = @"^[a-zA-Z0-9-\.\_]+$";
    public const string MoneyRegex = @"-?^\d+(\.\d{1,2})?$";
    public const string NonEmptyRegex = @"^.+$";

    // ---------------------------
    // Required Field Messages
    // ---------------------------
    public const string RequiredFieldMessage = "This field is required.";
    public const string RequiredDigitsMessage = "This field must contain digits or only.";
    public const string RequiredAlphaNumericMessage = "This field must contain letters, digits or - . _ only.";

    // ---------------------------
    // Warning / Validation Messages
    // ---------------------------
    public const string DigitsOnlyMessage = "Only digits are allowed.";
    public const string AlphaNumericOnlyMessage = "Only letters and digits or - . _ are allowed.";
    public const string MoneyFormatMessage = "Enter a valid amount (e.g., 12.34 or -12.34).";
    public const string RequiredLengthMessage = "Username must be between 3 and 50 characters long.";
    #endregion Constant Strings

    protected BaseViewModel(ICultureService culture, ILedgerKeyLookupFactory ledgerKeyLookup)
    {
        Culture = culture;
        LedgerKeyLookup = ledgerKeyLookup;
        Culture.CultureChanged += OnCultureChange;
        //Categories = CategoryLookup.AllCategories;
    }

    #region Category Popup
    [ObservableProperty]
    public partial LedgerEntry PickedLedgerKey { get; set; } = LedgerEntry.Empty;

    partial void OnPickedLedgerKeyChanged(LedgerEntry value)
    {
        if (value != LedgerEntry.Empty)
            IsDirty = true;
    }

    [ObservableProperty]
    public partial string LedgerKeySearchText { get; set; } = string.Empty;

    [ObservableProperty]
    public partial ObservableCollection<LedgerEntry> LedgerKeys { get; set; }  = [];

    [ObservableProperty]
    public partial ObservableCollection<LedgerEntry> FilteredLedgerKeys { get; set; } = [];

    partial void OnLedgerKeySearchTextChanged(string value)
    {
        ApplyFilter(value);
    }

    public async Task LoadLedgerKeysAsync()
    {
        if (LedgerKeyLookup.AllLedgerKeys.Count == 0)
            await LedgerKeyLookup.InitializeAsync();

        LedgerKeys.Clear();
        FilteredLedgerKeys.Clear();

        foreach (var c in LedgerKeyLookup.AllLedgerKeys)
        {
            var localized = Culture.TranslateLedgerKeyPath(c.Code);
            var entry = c with { LocalizedName = localized };

            LedgerKeys.Add(entry);
            FilteredLedgerKeys.Add(entry);
        }
    }

    private void ApplyFilter(string query)
    {
        FilteredLedgerKeys.Clear();

        foreach (var c in LedgerKeys)
        {
            if (string.IsNullOrWhiteSpace(query) ||
                c.LocalizedName.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                FilteredLedgerKeys.Add(c);
            }
        }
    }

    #endregion

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    public partial bool IsBusy { get; set; }
    public bool IsNotBusy => !IsBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotDirty))]
    public partial bool IsDirty { get; set; }
    public bool IsNotDirty => !IsDirty;

    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "Ready";
    public string LocalizedName { get; set; } = string.Empty;

    //public ObservableCollection<LedgerEntry> Categories { get; set; }
     private void OnCultureChange() { RefreshLocalizedName(); }

    protected virtual void RefreshLocalizedName() { }
    protected string TranslateLedgerKey(uint ledgerKeyCode) => Culture.TranslateLedgerKeyPath(ledgerKeyCode);
    protected void SetLocalizedName(string value) => LocalizedName = value;
    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName != nameof(IsDirty))
            IsDirty = true;
    }

    /// <summary>
    /// Validates all properties decorated with annotations before executing work.
    /// </summary>
    protected bool ValidateForm()
    {
        // Forces the toolkit to scan every property on the current page for validation errors
        ValidateAllProperties();

        if (HasErrors)
        {
            StatusMessage = "Validation Error: Please fix highlighted errors before saving.";
            return false;
        }
        return true;
    }

    protected async Task ExecuteSafeAsync(Func<Task> action, string busyMessage = "Processing...")
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            StatusMessage = busyMessage;
            //Logger.Debug($"Executing operation: '{busyMessage}'");
            await action();
            StatusMessage = "Ready";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            //Logger.Error(ex, $"Critical failure during context: '{StatusMessage}'");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

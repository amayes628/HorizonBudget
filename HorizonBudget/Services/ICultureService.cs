namespace  HorizonBudget.Services;

public interface ICultureService
{
    event Action? CultureChanged;

    string CurrentCulture { get; }

    Task InitializeAsync(string? culture = null);

    void SetCulture(string culture);

    string TranslateLedgerKey(uint code);

    string TranslateLedgerKeyPath(uint code);
}

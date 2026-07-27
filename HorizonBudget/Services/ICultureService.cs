namespace  HorizonBudget.Services;

public interface ICultureService
{
    string CurrentCultureCode { get; }

    event Action? CultureChanged;

    void ReloadTranslations();
    void SetCulture(string cultureCode);
    string TranslateCategory(uint categoryCode);
    string TranslateCategoryPath(uint categoryCode);
}

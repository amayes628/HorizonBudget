namespace  HorizonBudget.Services;

public interface IConfirmationService
{
    Task<bool> ConfirmAsync(
        XamlRoot root,
        string title,
        string message,
        string accept,
        string cancel);
}

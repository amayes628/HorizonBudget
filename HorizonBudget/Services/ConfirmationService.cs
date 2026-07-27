namespace HorizonBudget.Services;

public sealed partial class ConfirmationService : IConfirmationService
{
    public async Task<bool> ConfirmAsync(
        XamlRoot root,
        string title,
        string message,
        string accept,
        string cancel)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = message,
            PrimaryButtonText = accept,
            CloseButtonText = cancel,
            DefaultButton = ContentDialogButton.Primary,
            XamlRoot = root
        };

        var result = await dialog.ShowAsync();
        return result == ContentDialogResult.Primary;
    }
}

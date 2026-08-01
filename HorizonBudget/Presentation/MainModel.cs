
namespace HorizonBudget.Presentation;

public partial class MainModel : ObservableObject
{
    private readonly INavigator _navigator;

    [ObservableProperty]
    private string name = string.Empty;

    public MainModel(
        IStringLocalizer localizer,
        IOptions<AppConfig> appInfo,
        INavigator navigator)
    {
        _navigator = navigator;

        Title = $"Main - {localizer["ApplicationName"]} - {appInfo?.Value?.Environment}";
    }

    public string? Title { get; }

    [RelayCommand]
    public async Task GoToSecond()
    {
        await _navigator.NavigateViewModelAsync<SecondModel>(
            this,
            data: new Entity(Name)
        );
    }
}

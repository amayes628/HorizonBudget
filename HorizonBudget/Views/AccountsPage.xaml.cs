using HorizonBudget.Data;
using HorizonBudget.Data.Records;
using HorizonBudget.Services;
using HorizonBudget.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HorizonBudget.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class AccountsPage : Page
{
    // Add ViewModel property so the code-behind can reference it
    public AccountsViewModel ViewModel { get; private set; }

    public AccountsPage()
    {
        InitializeComponent();
        //ViewModel = App.GetService<AccountsViewModel>();
        //DataContext = ViewModel;

        Loaded += AccountsPage_Loaded;
    }

    private async void AccountsPage_Loaded(object sender, RoutedEventArgs e)
    {
        //if (ViewModel.Accounts.Count == 0)
        //{
        //    EmptyAccountsTip.IsOpen = true;
        //    await Task.Delay(5000);
        //    EmptyAccountsTip.IsOpen = false;
        //}
        // Safely grab the navigator assigned to this exact layout frame
        var regionNavigator = this.Navigator();

        // Fetch your service from the global container and hand-inject the navigator
        var repo = (Application.Current as App)!.Host!.Services.GetRequiredService<IRecordRepository<Account>>();
        var culture = (Application.Current as App)!.Host!.Services.GetRequiredService<ICultureService>();
        var lookup = (Application.Current as App)!.Host!.Services.GetRequiredService<ILedgerKeyLookupFactory>();

        DataContext = new AccountsViewModel(regionNavigator, repo, culture, lookup);
    }
}


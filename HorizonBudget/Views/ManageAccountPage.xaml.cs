using HorizonBudget.Data.Records;
using HorizonBudget.ViewModels;

namespace HorizonBudget.Views;

public sealed partial class ManageAccountPage : Page
{
    public ManageAccountViewModel ViewModel { get; }

    public ManageAccountPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<ManageAccountViewModel>();
        DataContext = ViewModel;
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        var account = e.Parameter as Account;
        ViewModel.Initialize(account);
    }
}

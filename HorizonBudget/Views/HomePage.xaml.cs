using HorizonBudget.ViewModels;
using HorizonBudget.Views.Pages;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HorizonBudget.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HomePage : Page
{
    public HomeViewModel? ViewModel => DataContext as HomeViewModel;
    public HomePage()
    {
        InitializeComponent();
        // Load default page

        mainpage.Navigate(typeof(DashboardPage));
    }
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);
        // Optionally, you can handle any initialization when the page is navigated to
        // The navigation pipeline sets DataContext to the VM it created.
        var ViewModel = DataContext as HomeViewModel;
    }

    private void NavView_SelectionChanged(NavigationView _, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem item)
            return;

        switch (item.Tag as string)
        {
            case "home":
            case "dashboard":
                mainpage.Navigate(typeof(DashboardPage));
                break;

            case "accounts":
                mainpage.Navigate(typeof(AccountsPage));
                break;

            case "expenses":
                // TODO: Add ExpensesPage
                break;

            case "income":
                // TODO: Add IncomePage
                break;

            case "about":
                // TODO: Add AboutPage
                break;

            case "help":
                // TODO: Add HelpPage
                break;

            case "settings":
                // TODO: Add SettingsPage
                break;
        }
    }
}

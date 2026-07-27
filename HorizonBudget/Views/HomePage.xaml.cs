using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace HorizonBudget.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class HomePage : Page
{
    public HomePage()
    {
        this.InitializeComponent();
        // Load default page
        ContentFrame.Navigate(typeof(DashboardPage));
    }
    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem item)
            return;

        switch (item.Tag as string)
        {
            case "home":
            case "dashboard":
                ContentFrame.Navigate(typeof(DashboardPage));
                break;

            case "accounts":
                //ContentFrame.Navigate(typeof(AccountsPage));
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

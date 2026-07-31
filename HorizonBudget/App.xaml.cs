using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using HorizonBudget.Data;
using HorizonBudget.Data.Records;
using HorizonBudget.Services;
using HorizonBudget.ViewModels;
using HorizonBudget.Views;
using HorizonBudget.Views.Pages;
using Microsoft.EntityFrameworkCore;
using Uno.UI.Extensions;

namespace HorizonBudget;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    public IHost? Host { get; private set; }

    [UnconditionalSuppressMessage(
        "Trimming", "IL2026",
        Justification = "Uno.Extensions.Localization is safe in HorizonBudget; required converters are preserved.")]
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Build host (logging, config, localization, DI, navigation)
        var builder = this.CreateBuilder(args)
            .Configure(host => host
#if DEBUG
                .UseEnvironment(Environments.Development)
#endif
                .UseLogging((context, logBuilder) =>
                {
                    logBuilder
                        .SetMinimumLevel(
                            context.HostingEnvironment.IsDevelopment() ?
                                LogLevel.Information :
                                LogLevel.Warning)
                        .CoreLogLevel(LogLevel.Warning);
                })
                .UseLocalization()
                .UseNavigation(RegisterRoutes)
                .UseHttp((context, services) =>
                {
#if DEBUG
                    services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif
                })
                .ConfigureServices((context, services) =>
                {
                    // EF Core
                    services.AddDbContextFactory<HorizonBudgetContext>(options =>
                    {
                        options.UseSqlite("Data Source=horizon.db");
                    });
                    services.AddTransient<DatabaseInitializer>();

                    // Services
                    services.AddSingleton<ICultureService, CultureService>();
                    services.AddSingleton<ILedgerKeyLookupFactory, LedgerKeyLookupFactory>();

                    // Repositories
                    services.AddScoped<IRecordRepository<Account>, RecordRepository<Account>>();
                    services.AddScoped<IRecordRepository<Expense>, RecordRepository<Expense>>();
                    services.AddScoped<IRecordRepository<Income>, RecordRepository<Income>>();
                    services.AddScoped<IRecordRepository<Transaction>, RecordRepository<Transaction>>();
                })
            );

        // Build the host once
        Host = builder.Build();
        
        // Create the window (builder.Window is available from the builder)
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif

        MainWindow.SetWindowIcon();

        // Initialize database
        using (var scope = Host.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            initializer.Initialize();
        }

        await builder.NavigateAsync<HomePage>();

        // Activate window after frame DataContext are set
        MainWindow.Activate();
    }


    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        Debug.WriteLine("RegisterRoutes being called");
        // 1. REGISTER VIEWS AND CORRESPONDING VIEWMODELS CLEANLY
        views.Register(
        new ViewMap<HomePage, HomeViewModel>(),
        new ViewMap<DashboardPage, DashboardViewModel>(),
        new ViewMap<AccountsPage, AccountsViewModel>(),
        new ViewMap<ManageAccountPage, ManageAccountViewModel>(),
        new DataViewMap<ManageAccountPage, ManageAccountViewModel, Account>(),
        new ViewMap<AboutPage>(),
        new ViewMap<HelpPage>(),
        new ViewMap<SettingsPage>()
            );

        Debug.WriteLine("RouteMap being initialized");
        // 2. DEFINE THE ROUTE PIPELINE & NESTED REGION HIERARCHY
        routes.Register(
        // The base application route maps to your root window host (HomeView)
        new RouteMap(
        "Home",
        View: views.FindByViewModel<HomeViewModel>(),
        Nested:
        [
                 // These routes populate the Frame inside HomePage's uen:Region
            new RouteMap("Dashboard", View: views.FindByViewModel<DashboardViewModel>(), IsDefault: true),
            new RouteMap("Accounts", View: views.FindByViewModel<AccountsViewModel>()),
            new RouteMap("ManageAccount", View: views.FindByViewModel<ManageAccountViewModel>()),
            new RouteMap("About", View: views.FindByView<AboutPage>()),
            new RouteMap("Help", View: views.FindByView<HelpPage>()),
            new RouteMap("Settings", View: views.FindByView<SettingsPage>())
        ]
        )
        );
    }

    public static T GetService<T>() where T : class =>
        ((App)Current).Host!.Services.GetRequiredService<T>();
}

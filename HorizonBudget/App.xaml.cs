using System.Diagnostics.CodeAnalysis;
using HorizonBudget.Data;
using HorizonBudget.Data.Records;
using HorizonBudget.Services;
using HorizonBudget.Views;
using Microsoft.EntityFrameworkCore;

namespace HorizonBudget;

public partial class App : Application
{
    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    protected Window? MainWindow { get; private set; }
    protected IHost? Host { get; private set; }

    [System.Diagnostics.CodeAnalysis.UnconditionalSuppressMessage(
    "Trimming", "IL2026",
    Justification = "Uno.Extensions.Localization is safe in HorizonBudget; required converters are preserved.")]
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Build host (logging, config, localization, DI)
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
                .UseHttp((context, services) =>
                {
#if DEBUG
                    services.AddTransient<DelegatingHandler, DebugHttpHandler>();
#endif
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContextFactory<HorizonBudgetContext>(options =>
                    {
                        options.UseSqlite("Data Source=horizon.db");
                    });
                    services.AddTransient<DatabaseInitializer>();
                    // Services
                    services.AddSingleton<ICultureService, CultureService>();
                    //services.AddSingleton<IRecordRepository<Account>, AccountRepository>();

                    // Factories
                    services.AddSingleton<CategoryLookupFactory>();
                })
            );

        // Create the window
        MainWindow = builder.Window;

#if DEBUG
        MainWindow.UseStudio();
#endif

        MainWindow.SetWindowIcon();
        // start up tasks
        // Build the app/ host
        var app = builder.Build();

        // Correct DI scope
        using (var scope = app.Services.CreateScope())
        {
            var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
            initializer.Initialize();
        }
        // YOUR ROOT UI PAGE
        var homePage = new HomePage();

        // Assign root page to window
        MainWindow.Content = homePage;

        // Activate window
        MainWindow.Activate();
    }

    private static void RegisterRoutes(IViewRegistry views, IRouteRegistry routes)
    {
        views.Register(
            new ViewMap(ViewModel: typeof(ShellModel)),
            new ViewMap<MainPage, MainModel>(),
            new DataViewMap<SecondPage, SecondModel, Entity>()
        );

        routes.Register(
            new RouteMap("", View: views.FindByViewModel<ShellModel>(),
                Nested:
                [
                    new ("Main", View: views.FindByViewModel<MainModel>(), IsDefault:true),
                    new ("Second", View: views.FindByViewModel<SecondModel>()),
                ]
            )
        );
    }
}

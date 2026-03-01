using ReactiveUI.Builder;
using ReactiveUI.Uno.SQLiteStudio.Presentation;
using ReactiveUI.Uno.SQLiteStudio.Services;
using ReactiveUI.Uno.SQLiteStudio.Views;

namespace ReactiveUI.Uno.SQLiteStudio;

/// <summary>
/// Represents the application entry point and manages application-level events and initialization logic for the app.
/// </summary>
/// <remarks>This class is responsible for configuring services, setting up the main window, and initializing the
/// application's navigation and data services when the app is launched. It extends the platform-specific Application
/// base class and is typically instantiated by the system at startup.</remarks>
public partial class App : Application
{
    private Window? _window;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    public App() => InitializeComponent();

    /// <summary>
    /// Handles application launch activation and initializes the main window and application services.
    /// </summary>
    /// <remarks>This method sets up dependency injection, registers application services, and navigates to
    /// the main view. It also ensures that sample data is seeded asynchronously after initialization. This override is
    /// typically called by the system when the application is launched by the user.</remarks>
    /// <param name="args">The launch activation arguments that provide details about how the application was launched.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        // Create the startup window if it doesn't exist
        _window ??= new Window();

        // Configure ReactiveUI with Uno and register services
        RxAppBuilder.CreateReactiveUIBuilder()

            // Breaking change in ReactiveUI.Uno 22+: WithUno now requires the startup window as a parameter
            .WithUno(_window)
            .WithDefaultIScreen()
            .WithRegistration(mutable =>
            {
                // Services
                mutable.RegisterLazySingleton(static () => SqliteService.Instance);
                mutable.RegisterLazySingleton(static () => CsvExportService.Instance);
            })
            .RegisterView<MainView, MainViewModel>()

            // Ensure to build and initialize the app dont use Build() as this only builds the locator
            .BuildApp()

            // Use WithInstance to get services registered in Splat.Applocator
            .WithInstance<IScreen, ISqliteService>((screen, sqlite) =>
            {
                // Navigate to Main
                screen!.Router.Navigate.Execute(new MainViewModel(screen)).Subscribe();

                // Seed sample data in background without blocking UI thread
                _ = InitializeDatabaseAsync(sqlite);

                _window.Content = new RoutedViewHost { Router = screen.Router };
                _window.Activate();
            });
    }

    private static async Task InitializeDatabaseAsync(ISqliteService? db)
    {
        if (db is null)
        {
            return;
        }

        await db.InitializeAsync().ConfigureAwait(false);
        await db.EnsureSampleDataAsync().ConfigureAwait(false);
    }
}

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Builder;
using ReactiveUI.Uno.SQLiteStudio.Presentation;
using ReactiveUI.Uno.SQLiteStudio.Services;
using ReactiveUI.Uno.SQLiteStudio.Views;

namespace ReactiveUI.Uno.SQLiteStudio;

public partial class App : Application
{
    private Window? _window;

    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() => InitializeComponent();

    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = _window ?? new Window();
        var appInstance = RxAppBuilder
            .CreateReactiveUIBuilder()
            .WithUno()
            .WithDefaultIScreen()
            .WithRegistration(mutable =>
            {
                // Services
                mutable.RegisterLazySingleton(static () => SqliteService.Instance);
                mutable.RegisterLazySingleton(static () => CsvExportService.Instance);

                // Seed sample data
                SqliteService.Instance.InitializeAsync().GetAwaiter().GetResult();
                SqliteService.Instance.EnsureSampleDataAsync().GetAwaiter().GetResult();
            })
            .RegisterView<MainView, MainViewModel>()
            .BuildApp()
            .WithInstance<IScreen>(screen => { 
                // Navigate to Main
                screen!.Router.Navigate.Execute(new MainViewModel(screen)).Subscribe();
                var host = new RoutedViewHost { Router = screen.Router };
                _window.Content = host;
                _window.Activate();
            });
    }
}

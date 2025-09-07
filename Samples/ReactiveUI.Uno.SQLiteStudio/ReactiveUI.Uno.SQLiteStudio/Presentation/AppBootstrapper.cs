using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Uno.SQLiteStudio.Services;

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

public class AppBootstrapper : ReactiveObject, IScreen
{
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public AppBootstrapper()
    {
        Router = new RoutingState();

        // Services
        Locator.CurrentMutable.RegisterLazySingleton<ISqliteService>(() => SqliteService.Instance);
        Locator.CurrentMutable.RegisterLazySingleton<ICsvExportService>(() => CsvExportService.Instance);

        // Views
        Locator.CurrentMutable.Register(() => new Views.MainView(), typeof(IViewFor<MainViewModel>));

        // Seed sample data
        SqliteService.Instance.InitializeAsync().GetAwaiter().GetResult();
        SqliteService.Instance.EnsureSampleDataAsync().GetAwaiter().GetResult();

        // Navigate to Main
        Router.Navigate.Execute(new MainViewModel(this)).Subscribe();
    }

    public RoutingState Router { get; }
}

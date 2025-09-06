using System;
using ReactiveUI;
using Splat;
using ReactiveUI.Uno.SQLiteStudio.Services;

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

public class AppBootstrapper : ReactiveObject, IScreen
{
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

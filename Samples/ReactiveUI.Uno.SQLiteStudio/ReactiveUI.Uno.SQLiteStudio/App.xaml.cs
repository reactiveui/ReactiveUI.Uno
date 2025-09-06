namespace ReactiveUI.Uno.SQLiteStudio;

public partial class App : Application
{
    private Window? _window;

    /// <summary>
    /// Initializes the singleton application object. This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App() => InitializeComponent();

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window = _window ?? new Window();
        var bootstrapper = new Presentation.AppBootstrapper();
        var host = new RoutedViewHost { Router = bootstrapper.Router };
        _window.Content = host;
        _window.Activate();
    }
}

using Uno.UI.Hosting;

namespace ReactiveUI.Uno.SQLiteStudio;

/// <summary>
/// Provides the entry point for the application.
/// </summary>
/// <remarks>This class contains the Main method, which initializes and runs the Uno Platform application. The
/// application is configured to use WebAssembly as the runtime environment.</remarks>
public class Program
{
    /// <summary>
    /// Initializes and runs the Uno Platform WebAssembly application.
    /// </summary>
    /// <remarks>This is the entry point of the application when running as a WebAssembly app. The method
    /// configures and starts the Uno Platform host using the specified application class.</remarks>
    /// <param name="args">An array of command-line arguments supplied to the application.</param>
    /// <returns>A task that represents the asynchronous operation of running the application.</returns>
    public static async Task Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseWebAssembly()
            .Build();

        await host.RunAsync();
    }
}

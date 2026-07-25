// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Uno.UI.Hosting;

namespace ReactiveUI.Uno.SQLiteStudio.iOS;

/// <summary>
/// Provides the main entry point for the application.
/// </summary>
/// <remarks>This class contains the application's startup logic. The runtime invokes the Main method to
/// initialize and run the application.</remarks>
public class EntryPoint
{
    /// <summary>
    /// Serves as the entry point for the application.
    /// </summary>
    /// <remarks>This method initializes and runs the Uno Platform application using the Apple UIKit backend.
    /// It is typically called automatically by the runtime and should not be invoked directly.</remarks>
    /// <param name="args">An array of command-line arguments supplied to the application.</param>
    public static void Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseAppleUIKit()
            .Build();

        host.Run();
    }
}

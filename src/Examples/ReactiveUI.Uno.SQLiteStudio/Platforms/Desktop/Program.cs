// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Uno.UI.Hosting;

namespace ReactiveUI.Uno.SQLiteStudio;

/// <summary>Provides the desktop application entry point.</summary>
internal static class Program
{
    /// <summary>Runs the desktop host.</summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>A task that completes when the desktop host exits.</returns>
    [STAThread]
    private static async Task Main(string[] args)
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new App())
            .UseWin32()
            .UseMacOS()
            .UseX11()
            .UseLinuxFrameBuffer()
            .Build();

        await host.RunAsync().ConfigureAwait(true);
    }
}

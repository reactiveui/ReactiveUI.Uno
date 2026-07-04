// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Uno.UI.Hosting;

namespace ReactiveUI.Uno.Reactive.IoTDashboard;

/// <summary>Provides the desktop host entry point.</summary>
internal static class Program
{
    /// <summary>Runs the desktop host.</summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>A task that completes when the desktop host exits.</returns>
    [STAThread]
    public static async Task Main(string[] args)
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

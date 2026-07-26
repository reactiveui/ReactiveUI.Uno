// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Uno.Reactive.IoTDashboard.Services;
using ReactiveUI.Uno.Reactive.IoTDashboard.ViewModels;
using ReactiveUI.Uno.Reactive.IoTDashboard.Views;

namespace ReactiveUI.Uno.Reactive.IoTDashboard;

/// <summary>Provides the Uno application entry point for the reactive IoT dashboard sample.</summary>
public partial class App : Application
{
    /// <summary>Stores the application window.</summary>
    private Window? _window;

    /// <summary>Tracks whether application services were already initialized.</summary>
    private bool _isInitialized;

    /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
    public App() => InitializeComponent();

    /// <summary>Configures ReactiveUI and opens the dashboard view when the app is launched.</summary>
    /// <param name="args">The launch activation arguments.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            _window ??= new Window();
            if (!_isInitialized)
            {
                InitializeApplication(_window);
                _isInitialized = true;
            }

            _window.Activate();
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            throw;
        }
    }

    /// <summary>Configures ReactiveUI.Uno.Reactive services and hosts the dashboard view.</summary>
    /// <param name="window">The startup window that hosts the dashboard.</param>
    private static void InitializeApplication(Window window)
    {
        var screen = new AppBootstrapper();
        var timeProvider = TimeProvider.System;
        var telemetry = new LocalIoTTelemetryService(timeProvider);

        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithUno(window)
            .WithRegistration(mutable =>
            {
                mutable.RegisterConstant<IScreen>(screen);
                mutable.RegisterConstant(timeProvider);
                mutable.RegisterConstant<IIoTTelemetryService>(telemetry);
            })
            .BuildApp();

        var viewModel = new DashboardViewModel(screen, telemetry, timeProvider);

        window.Content = new DashboardView { ViewModel = viewModel };
    }
}

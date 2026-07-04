// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Builder;
using ReactiveUI.Uno.SQLiteStudio.Presentation;
using ReactiveUI.Uno.SQLiteStudio.Services;
using ReactiveUI.Uno.SQLiteStudio.Views;

namespace ReactiveUI.Uno.SQLiteStudio;

/// <summary>Represents the application entry point and manages application-level events and initialization logic for the app.</summary>
/// <remarks>This class is responsible for configuring services, setting up the main window, and initializing the
/// application's navigation and data services when the app is launched. It extends the platform-specific Application
/// base class and is typically instantiated by the system at startup.</remarks>
public partial class App : Application
{
    /// <summary>Stores the application window.</summary>
    private Window? _window;

    /// <summary>Tracks whether the application services and initial route have already been configured.</summary>
    private bool _isInitialized;

    /// <summary>Initializes a new instance of the <see cref="App"/> class.</summary>
    public App() => InitializeComponent();

    /// <summary>Handles application launch activation and initializes the main window and application services.</summary>
    /// <remarks>This method sets up dependency injection, registers application services, and navigates to
    /// the main view. It also ensures that sample data is seeded asynchronously after initialization. This override is
    /// typically called by the system when the application is launched by the user.</remarks>
    /// <param name="args">The launch activation arguments that provide details about how the application was launched.</param>
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        try
        {
            _window ??= new Window();
            if (!_isInitialized)
            {
                var startup = InitializeApplication(_window);
                _isInitialized = true;
                _window.Activate();
                _ = InitializeDatabaseAsync(startup.Database, startup.ViewModel);
            }
            else
            {
                _window.Activate();
            }
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            throw;
        }
    }

    /// <summary>Initializes the application database and sample data.</summary>
    /// <param name="db">The SQLite service to initialize.</param>
    /// <param name="viewModel">The main view model to update with startup state.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    private static async Task InitializeDatabaseAsync(ISqliteService db, MainViewModel viewModel)
    {
        try
        {
            viewModel.Status = "Initializing database...";
            await db.InitializeAsync().ConfigureAwait(true);
            await db.EnsureSampleDataAsync().ConfigureAwait(true);
            viewModel.Status = "Ready";
        }
        catch (Exception exception)
        {
            Debug.WriteLine(exception);
            viewModel.Status = exception.Message;
        }
    }

    /// <summary>Configures ReactiveUI and hosts the main reactive view.</summary>
    /// <param name="window">The startup window that hosts the main view.</param>
    /// <returns>The startup services that are needed after the window has activated.</returns>
    private static (ISqliteService Database, MainViewModel ViewModel) InitializeApplication(Window window)
    {
        var screen = new AppBootstrapper();
        var sqlite = SqliteService.Instance;
        var csv = CsvExportService.Instance;

        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithUno(window)
            .WithRegistration(mutable =>
            {
                mutable.RegisterConstant<IScreen>(screen);
                mutable.RegisterConstant(sqlite);
                mutable.RegisterConstant(csv);
            })
            .BuildApp();

        var viewModel = new MainViewModel(screen, sqlite, csv);

        window.Content = new MainView
        {
            ViewModel = viewModel
        };

        return (sqlite, viewModel);
    }
}

// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Bootstrapping;

/// <summary>Contains tests for the <see cref="AppBootstrapper"/> class, ensuring its functionality for application bootstrapping with ReactiveUI.</summary>
public class AppBootstrapperTests
{
    /// <summary>Validates that AppBootstrapper can be instantiated without errors.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Constructor_CreatesInstance_Successfully()
    {
        AppBootstrapper? bootstrapper = new();
        await Assert.That(bootstrapper).IsNotNull();
    }

    /// <summary>Validates that AppBootstrapper implements IScreen interface.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task AppBootstrapper_ImplementsIScreen()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsAssignableTo<IScreen>();
    }

    /// <summary>Validates that Router property is initialized and not null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Router_IsInitialized_NotNull()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper.Router).IsNotNull();
        await Assert.That(bootstrapper.Router).IsAssignableTo<RoutingState>();
    }

    /// <summary>Validates that AppBootstrapper inherits from ReactiveObject.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task AppBootstrapper_InheritsFromReactiveObject()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsAssignableTo<ReactiveObject>();
    }

    /// <summary>Validates that multiple instances can be created.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Constructor_AllowsMultipleInstances()
    {
        var bootstrapper1 = new AppBootstrapper();
        var bootstrapper2 = new AppBootstrapper();

        await Assert.That(bootstrapper1).IsNotNull();
        await Assert.That(bootstrapper2).IsNotNull();
        await Assert.That(bootstrapper1).IsNotSameReferenceAs(bootstrapper2);
        await Assert.That(bootstrapper1.Router).IsNotSameReferenceAs(bootstrapper2.Router);
    }

    /// <summary>Validates that Router navigation stack is initially empty.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Router_NavigationStack_IsInitiallyEmpty()
    {
        var bootstrapper = new AppBootstrapper();

        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsZero();
    }

    /// <summary>Validates that Router can navigate to a view model.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Router_CanNavigate_ToViewModel()
    {
        var bootstrapper = new AppBootstrapper();
        var viewModel = new TestRoutableViewModel(bootstrapper);

        await ExecuteNavigationAsync(bootstrapper.Router.Navigate.Execute(viewModel));

        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>Validates that Router CurrentViewModel updates after navigation.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Router_CurrentViewModel_UpdatesAfterNavigation()
    {
        var bootstrapper = new AppBootstrapper();
        var viewModel = new TestRoutableViewModel(bootstrapper);

        await ExecuteNavigationAsync(bootstrapper.Router.Navigate.Execute(viewModel));

        var currentVm = bootstrapper.Router.NavigationStack.LastOrDefault();
        await Assert.That(currentVm).IsEqualTo(viewModel);
    }

    /// <summary>Validates that Router can navigate back.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Router_CanNavigate_Back()
    {
        var bootstrapper = new AppBootstrapper();
        var viewModel1 = new TestRoutableViewModel(bootstrapper);
        var viewModel2 = new TestRoutableViewModel(bootstrapper);

        await ExecuteNavigationAsync(bootstrapper.Router.Navigate.Execute(viewModel1));
        await ExecuteNavigationAsync(bootstrapper.Router.Navigate.Execute(viewModel2));
        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(2);

        await ExecuteNavigationAsync(bootstrapper.Router.NavigateBack.Execute());

        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>Validates that IScreen.Router returns the same RoutingState instance.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task IScreen_Router_ReturnsSameInstance()
    {
        var bootstrapper = new AppBootstrapper();
        var screenRouter = ((IScreen)bootstrapper).Router;

        await Assert.That(screenRouter).IsEqualTo(bootstrapper.Router);
    }

    /// <summary>Validates that AppBootstrapper implements IReactiveObject.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task AppBootstrapper_ImplementsIReactiveObject()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsAssignableTo<IReactiveObject>();
    }

    /// <summary>Validates that Router NavigateAndReset clears the navigation stack.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Router_NavigateAndReset_ClearsNavigationStack()
    {
        var bootstrapper = new AppBootstrapper();
        var viewModel1 = new TestRoutableViewModel(bootstrapper);
        var viewModel2 = new TestRoutableViewModel(bootstrapper);
        var viewModel3 = new TestRoutableViewModel(bootstrapper);

        await ExecuteNavigationAsync(bootstrapper.Router.Navigate.Execute(viewModel1));
        await ExecuteNavigationAsync(bootstrapper.Router.Navigate.Execute(viewModel2));
        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(2);

        await ExecuteNavigationAsync(bootstrapper.Router.NavigateAndReset.Execute(viewModel3));

        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>Executes the supplied navigation observable and returns when it completes.</summary>
    /// <typeparam name="T">The navigation result type.</typeparam>
    /// <param name="navigation">The navigation observable to execute.</param>
    /// <returns>A task that completes when navigation completes.</returns>
    private static Task ExecuteNavigationAsync<T>(IObservable<T> navigation)
    {
        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var subscription = new SerialDisposable();

        void CompleteSuccessfully()
        {
            _ = completion.TrySetResult();
            subscription.Dispose();
        }

        void CompleteWithError(Exception exception)
        {
            _ = completion.TrySetException(exception);
            subscription.Dispose();
        }

        subscription.Disposable = navigation.Subscribe(
            _ => CompleteSuccessfully(),
            CompleteWithError,
            CompleteSuccessfully);

        return completion.Task;
    }

    /// <summary>Test routable view model for testing navigation.</summary>
    private sealed class TestRoutableViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <summary>Initializes a new instance of the <see cref="TestRoutableViewModel"/> class.</summary>
        /// <param name="hostScreen">The host screen used by the routable view model.</param>
        public TestRoutableViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }

        /// <inheritdoc/>
        public string UrlPathSegment => "test";

        /// <inheritdoc/>
        public IScreen HostScreen { get; }
    }
}

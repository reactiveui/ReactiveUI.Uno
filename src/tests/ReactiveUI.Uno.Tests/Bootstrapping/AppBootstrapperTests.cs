// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Bootstrapping;

/// <summary>
/// Contains tests for the <see cref="AppBootstrapper"/> class, ensuring its functionality
/// for application bootstrapping with ReactiveUI.
/// </summary>
public class AppBootstrapperTests
{
    /// <summary>
    /// Validates that AppBootstrapper can be instantiated without errors.
    /// </summary>
    [Test]
    public async Task Constructor_CreatesInstance_Successfully()
    {
        AppBootstrapper? bootstrapper = null;

        bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsNotNull();
    }

    /// <summary>
    /// Validates that AppBootstrapper implements IScreen interface.
    /// </summary>
    [Test]
    public async Task AppBootstrapper_ImplementsIScreen()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsAssignableTo<IScreen>();
    }

    /// <summary>
    /// Validates that Router property is initialized and not null.
    /// </summary>
    [Test]
    public async Task Router_IsInitialized_NotNull()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper.Router).IsNotNull();
        await Assert.That(bootstrapper.Router).IsAssignableTo<RoutingState>();
    }

    /// <summary>
    /// Validates that AppBootstrapper inherits from ReactiveObject.
    /// </summary>
    [Test]
    public async Task AppBootstrapper_InheritsFromReactiveObject()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsAssignableTo<ReactiveObject>();
    }

    /// <summary>
    /// Validates that multiple instances can be created.
    /// </summary>
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

    /// <summary>
    /// Validates that Router navigation stack is initially empty.
    /// </summary>
    [Test]
    public async Task Router_NavigationStack_IsInitiallyEmpty()
    {
        var bootstrapper = new AppBootstrapper();

        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsZero();
    }

    /// <summary>
    /// Validates that Router can navigate to a view model.
    /// </summary>
    [Test]
    public async Task Router_CanNavigate_ToViewModel()
    {
        var bootstrapper = new AppBootstrapper();
        var viewModel = new TestRoutableViewModel(bootstrapper);

        await bootstrapper.Router.Navigate.Execute(viewModel);

        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>
    /// Validates that Router CurrentViewModel updates after navigation.
    /// </summary>
    [Test]
    public async Task Router_CurrentViewModel_UpdatesAfterNavigation()
    {
        var bootstrapper = new AppBootstrapper();
        var viewModel = new TestRoutableViewModel(bootstrapper);

        await bootstrapper.Router.Navigate.Execute(viewModel);

        var currentVm = await bootstrapper.Router.CurrentViewModel.FirstAsync();
        await Assert.That(currentVm).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that Router can navigate back.
    /// </summary>
    [Test]
    public async Task Router_CanNavigate_Back()
    {
        var bootstrapper = new AppBootstrapper();
        var viewModel1 = new TestRoutableViewModel(bootstrapper);
        var viewModel2 = new TestRoutableViewModel(bootstrapper);

        await bootstrapper.Router.Navigate.Execute(viewModel1);
        await bootstrapper.Router.Navigate.Execute(viewModel2);
        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(2);

        await bootstrapper.Router.NavigateBack.Execute();

        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>
    /// Validates that IScreen.Router returns the same RoutingState instance.
    /// </summary>
    [Test]
    public async Task IScreen_Router_ReturnsSameInstance()
    {
        var bootstrapper = new AppBootstrapper();
        var screenRouter = ((IScreen)bootstrapper).Router;

        await Assert.That(screenRouter).IsEqualTo(bootstrapper.Router);
    }

    /// <summary>
    /// Validates that AppBootstrapper implements IReactiveObject.
    /// </summary>
    [Test]
    public async Task AppBootstrapper_ImplementsIReactiveObject()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsAssignableTo<IReactiveObject>();
    }

    /// <summary>
    /// Validates that Router NavigateAndReset clears the navigation stack.
    /// </summary>
    [Test]
    public async Task Router_NavigateAndReset_ClearsNavigationStack()
    {
        var bootstrapper = new AppBootstrapper();
        var viewModel1 = new TestRoutableViewModel(bootstrapper);
        var viewModel2 = new TestRoutableViewModel(bootstrapper);
        var viewModel3 = new TestRoutableViewModel(bootstrapper);

        await bootstrapper.Router.Navigate.Execute(viewModel1);
        await bootstrapper.Router.Navigate.Execute(viewModel2);
        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(2);

        await bootstrapper.Router.NavigateAndReset.Execute(viewModel3);

        await Assert.That(bootstrapper.Router.NavigationStack.Count).IsEqualTo(1);
    }

    /// <summary>
    /// Test routable view model for testing navigation.
    /// </summary>
    private sealed class TestRoutableViewModel : ReactiveObject, IRoutableViewModel
    {
        public TestRoutableViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }

        public string UrlPathSegment => "test";

        public IScreen HostScreen { get; }
    }
}

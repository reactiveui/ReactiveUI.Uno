// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Controls;

/// <summary>
/// Contains tests for the <see cref="ReactivePage{TViewModel}"/> class.
/// </summary>
public class ReactivePageTests
{
    /// <summary>
    /// Setup for each test - skip if no UI context is available.
    /// </summary>
    [Before(Test)]
    public void SetUp()
    {
        try
        {
            _ = Window.Current;
        }
        catch (TypeInitializationException)
        {
            Skip.Test("Skipping test because no UI context is available (headless environment)");
        }
        catch (NotSupportedException)
        {
            Skip.Test("Skipping test because no UI context is available (headless environment)");
        }
    }

    /// <summary>
    /// Validates that TestReactivePage can be instantiated.
    /// </summary>
    [Test]
    public async Task ReactivePage_CanBeInstantiated()
    {
        var page = new TestReactivePage();
        await Assert.That(page).IsNotNull();
    }

    /// <summary>
    /// Validates that ReactivePage implements IViewFor interface.
    /// </summary>
    [Test]
    public async Task ReactivePage_ImplementsIViewFor()
    {
        var page = new TestReactivePage();
        await Assert.That(page).IsAssignableTo<IViewFor<TestPageViewModel>>();
    }

    /// <summary>
    /// Validates that ReactivePage implements non-generic IViewFor interface.
    /// </summary>
    [Test]
    public async Task ReactivePage_ImplementsNonGenericIViewFor()
    {
        var page = new TestReactivePage();
        await Assert.That(page).IsAssignableTo<IViewFor>();
    }

    /// <summary>
    /// Validates that ReactivePage inherits from Page.
    /// </summary>
    [Test]
    public async Task ReactivePage_InheritsFromPage()
    {
        var page = new TestReactivePage();
        await Assert.That(page).IsAssignableTo<Page>();
    }

    /// <summary>
    /// Validates that ViewModel property is initially null.
    /// </summary>
    [Test]
    public async Task ViewModel_IsInitiallyNull()
    {
        var page = new TestReactivePage();
        await Assert.That(page.ViewModel).IsNull();
    }

    /// <summary>
    /// Validates that ViewModel property can be set.
    /// </summary>
    [Test]
    public async Task ViewModel_CanBeSet()
    {
        var page = new TestReactivePage();
        var viewModel = new TestPageViewModel();
        page.ViewModel = viewModel;
        await Assert.That(page.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that ViewModel property can be set to null.
    /// </summary>
    [Test]
    public async Task ViewModel_CanBeSetToNull()
    {
        var page = new TestReactivePage();
        page.ViewModel = new TestPageViewModel();
        page.ViewModel = null;
        await Assert.That(page.ViewModel).IsNull();
    }

    /// <summary>
    /// Validates that BindingRoot returns the ViewModel.
    /// </summary>
    [Test]
    public async Task BindingRoot_ReturnsViewModel()
    {
        var page = new TestReactivePage();
        var viewModel = new TestPageViewModel();
        page.ViewModel = viewModel;
        await Assert.That(page.BindingRoot).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that BindingRoot returns null when ViewModel is null.
    /// </summary>
    [Test]
    public async Task BindingRoot_ReturnsNull_WhenViewModelIsNull()
    {
        var page = new TestReactivePage();
        await Assert.That(page.BindingRoot).IsNull();
    }

    /// <summary>
    /// Validates that non-generic IViewFor.ViewModel property works correctly.
    /// </summary>
    [Test]
    public async Task IViewForViewModel_GetterWorks()
    {
        var page = new TestReactivePage();
        var viewModel = new TestPageViewModel();
        page.ViewModel = viewModel;
        var nonGenericViewModel = ((IViewFor)page).ViewModel;
        await Assert.That(nonGenericViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that non-generic IViewFor.ViewModel setter works correctly.
    /// </summary>
    [Test]
    public async Task IViewForViewModel_SetterWorks()
    {
        var page = new TestReactivePage();
        var viewModel = new TestPageViewModel();
        ((IViewFor)page).ViewModel = viewModel;
        await Assert.That(page.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that non-generic IViewFor.ViewModel can be set to null.
    /// </summary>
    [Test]
    public async Task IViewForViewModel_CanBeSetToNull()
    {
        var page = new TestReactivePage();
        page.ViewModel = new TestPageViewModel();
        ((IViewFor)page).ViewModel = null;
        await Assert.That(page.ViewModel).IsNull();
    }

    /// <summary>
    /// Validates that ViewModelProperty DependencyProperty exists.
    /// </summary>
    [Test]
    public async Task ViewModelProperty_DependencyProperty_Exists()
    {
        await Assert.That(ReactivePage<TestPageViewModel>.ViewModelProperty).IsNotNull();
    }

    /// <summary>
    /// Validates that multiple instances can be created independently.
    /// </summary>
    [Test]
    public async Task MultipleInstances_AreIndependent()
    {
        var page1 = new TestReactivePage();
        var page2 = new TestReactivePage();
        var viewModel1 = new TestPageViewModel();
        var viewModel2 = new TestPageViewModel();
        page1.ViewModel = viewModel1;
        page2.ViewModel = viewModel2;
        await Assert.That(page1.ViewModel).IsNotSameReferenceAs(page2.ViewModel);
        await Assert.That(page1.ViewModel).IsEqualTo(viewModel1);
        await Assert.That(page2.ViewModel).IsEqualTo(viewModel2);
    }

    /// <summary>
    /// Validates that ViewModel can be updated multiple times.
    /// </summary>
    [Test]
    public async Task ViewModel_CanBeUpdatedMultipleTimes()
    {
        var page = new TestReactivePage();
        var viewModel1 = new TestPageViewModel();
        var viewModel2 = new TestPageViewModel();
        page.ViewModel = viewModel1;
        await Assert.That(page.ViewModel).IsEqualTo(viewModel1);
        page.ViewModel = viewModel2;
        await Assert.That(page.ViewModel).IsEqualTo(viewModel2);
    }

    /// <summary>
    /// Test view model for testing purposes.
    /// </summary>
    public sealed class TestPageViewModel
    {
    }

    /// <summary>
    /// Test ReactivePage implementation for testing purposes.
    /// </summary>
    public sealed class TestReactivePage : ReactivePage<TestPageViewModel>
    {
    }
}

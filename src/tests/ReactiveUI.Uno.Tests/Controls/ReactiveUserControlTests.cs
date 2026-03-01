// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Controls;

/// <summary>
/// Contains tests for the <see cref="ReactiveUserControl{TViewModel}"/> class.
/// </summary>
public class ReactiveUserControlTests
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
    /// Validates that TestReactiveUserControl can be instantiated.
    /// </summary>
    [Test]
    public async Task ReactiveUserControl_CanBeInstantiated()
    {
        var control = new TestReactiveUserControl();
        await Assert.That(control).IsNotNull();
    }

    /// <summary>
    /// Validates that ReactiveUserControl implements IViewFor interface.
    /// </summary>
    [Test]
    public async Task ReactiveUserControl_ImplementsIViewFor()
    {
        var control = new TestReactiveUserControl();
        await Assert.That(control).IsAssignableTo<IViewFor<TestViewModel>>();
    }

    /// <summary>
    /// Validates that ReactiveUserControl implements non-generic IViewFor interface.
    /// </summary>
    [Test]
    public async Task ReactiveUserControl_ImplementsNonGenericIViewFor()
    {
        var control = new TestReactiveUserControl();
        await Assert.That(control).IsAssignableTo<IViewFor>();
    }

    /// <summary>
    /// Validates that ReactiveUserControl inherits from UserControl.
    /// </summary>
    [Test]
    public async Task ReactiveUserControl_InheritsFromUserControl()
    {
        var control = new TestReactiveUserControl();
        await Assert.That(control).IsAssignableTo<UserControl>();
    }

    /// <summary>
    /// Validates that ViewModel property is initially null.
    /// </summary>
    [Test]
    public async Task ViewModel_IsInitiallyNull()
    {
        var control = new TestReactiveUserControl();
        await Assert.That(control.ViewModel).IsNull();
    }

    /// <summary>
    /// Validates that ViewModel property can be set.
    /// </summary>
    [Test]
    public async Task ViewModel_CanBeSet()
    {
        var control = new TestReactiveUserControl();
        var viewModel = new TestViewModel();
        control.ViewModel = viewModel;
        await Assert.That(control.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that ViewModel property can be set to null.
    /// </summary>
    [Test]
    public async Task ViewModel_CanBeSetToNull()
    {
        var control = new TestReactiveUserControl();
        control.ViewModel = new TestViewModel();
        control.ViewModel = null;
        await Assert.That(control.ViewModel).IsNull();
    }

    /// <summary>
    /// Validates that BindingRoot returns the ViewModel.
    /// </summary>
    [Test]
    public async Task BindingRoot_ReturnsViewModel()
    {
        var control = new TestReactiveUserControl();
        var viewModel = new TestViewModel();
        control.ViewModel = viewModel;
        await Assert.That(control.BindingRoot).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that BindingRoot returns null when ViewModel is null.
    /// </summary>
    [Test]
    public async Task BindingRoot_ReturnsNull_WhenViewModelIsNull()
    {
        var control = new TestReactiveUserControl();
        await Assert.That(control.BindingRoot).IsNull();
    }

    /// <summary>
    /// Validates that non-generic IViewFor.ViewModel property works correctly.
    /// </summary>
    [Test]
    public async Task IViewForViewModel_GetterWorks()
    {
        var control = new TestReactiveUserControl();
        var viewModel = new TestViewModel();
        control.ViewModel = viewModel;
        var nonGenericViewModel = ((IViewFor)control).ViewModel;
        await Assert.That(nonGenericViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that non-generic IViewFor.ViewModel setter works correctly.
    /// </summary>
    [Test]
    public async Task IViewForViewModel_SetterWorks()
    {
        var control = new TestReactiveUserControl();
        var viewModel = new TestViewModel();
        ((IViewFor)control).ViewModel = viewModel;
        await Assert.That(control.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that non-generic IViewFor.ViewModel can be set to null.
    /// </summary>
    [Test]
    public async Task IViewForViewModel_CanBeSetToNull()
    {
        var control = new TestReactiveUserControl();
        control.ViewModel = new TestViewModel();
        ((IViewFor)control).ViewModel = null;
        await Assert.That(control.ViewModel).IsNull();
    }

    /// <summary>
    /// Validates that ViewModelProperty DependencyProperty exists.
    /// </summary>
    [Test]
    public async Task ViewModelProperty_DependencyProperty_Exists()
    {
        await Assert.That(ReactiveUserControl<TestViewModel>.ViewModelProperty).IsNotNull();
    }

    /// <summary>
    /// Validates that multiple instances can be created independently.
    /// </summary>
    [Test]
    public async Task MultipleInstances_AreIndependent()
    {
        var control1 = new TestReactiveUserControl();
        var control2 = new TestReactiveUserControl();
        var viewModel1 = new TestViewModel();
        var viewModel2 = new TestViewModel();
        control1.ViewModel = viewModel1;
        control2.ViewModel = viewModel2;
        await Assert.That(control1.ViewModel).IsNotSameReferenceAs(control2.ViewModel);
        await Assert.That(control1.ViewModel).IsEqualTo(viewModel1);
        await Assert.That(control2.ViewModel).IsEqualTo(viewModel2);
    }

    /// <summary>
    /// Test view model for testing purposes.
    /// </summary>
    public sealed class TestViewModel
    {
    }

    /// <summary>
    /// Test ReactiveUserControl implementation for testing purposes.
    /// </summary>
    public sealed class TestReactiveUserControl : ReactiveUserControl<TestViewModel>
    {
    }
}

// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using Splat;
using TUnit.Assertions.Extensions;
using TUnit.Core;
using RxObservable = System.Reactive.Linq.Observable;

namespace ReactiveUI.Uno.Tests.Controls;

/// <summary>
/// Contains tests for the <see cref="ViewModelViewHost"/> class.
/// </summary>
public class ViewModelViewHostTests
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
    /// Validates that ViewModelViewHost can be instantiated.
    /// </summary>
    [Test]
    public async Task ViewModelViewHost_CanBeInstantiated()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host).IsNotNull();
    }

    /// <summary>
    /// Validates that ViewModelViewHost implements IViewFor.
    /// </summary>
    [Test]
    public async Task ViewModelViewHost_ImplementsIViewFor()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host).IsAssignableTo<IViewFor>();
    }

    /// <summary>
    /// Validates that ViewModelViewHost implements IEnableLogger.
    /// </summary>
    [Test]
    public async Task ViewModelViewHost_ImplementsIEnableLogger()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host).IsAssignableTo<IEnableLogger>();
    }

    /// <summary>
    /// Validates that ViewModelViewHost inherits from TransitioningContentControl.
    /// </summary>
    [Test]
    public async Task ViewModelViewHost_InheritsFromTransitioningContentControl()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host).IsAssignableTo<TransitioningContentControl>();
    }

    /// <summary>
    /// Validates that ViewModel property is initially null.
    /// </summary>
    [Test]
    public async Task ViewModel_IsInitiallyNull()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host.ViewModel).IsNull();
    }

    /// <summary>
    /// Validates that ViewModel property can be set.
    /// </summary>
    [Test]
    public async Task ViewModel_CanBeSet()
    {
        var host = new ViewModelViewHost();
        var viewModel = new TestViewModel();
        host.ViewModel = viewModel;
        await Assert.That(host.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Validates that DefaultContent property is initially null.
    /// </summary>
    [Test]
    public async Task DefaultContent_IsInitiallyNull()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host.DefaultContent).IsNull();
    }

    /// <summary>
    /// Validates that DefaultContent property can be set.
    /// </summary>
    [Test]
    public async Task DefaultContent_CanBeSet()
    {
        var host = new ViewModelViewHost();
        var content = new object();
        host.DefaultContent = content;
        await Assert.That(host.DefaultContent).IsEqualTo(content);
    }

    /// <summary>
    /// Validates that ViewContract property is initially null.
    /// </summary>
    [Test]
    public async Task ViewContract_IsInitiallyNull()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host.ViewContract).IsNull();
    }

    /// <summary>
    /// Validates that ViewContract property can be set.
    /// </summary>
    [Test]
    public async Task ViewContract_CanBeSet()
    {
        var host = new ViewModelViewHost();
        host.ViewContract = "TestContract";
        await Assert.That(host.ViewContract).IsEqualTo("TestContract");
    }

    /// <summary>
    /// Validates that ViewLocator property is initially null.
    /// </summary>
    [Test]
    public async Task ViewLocator_IsInitiallyNull()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host.ViewLocator).IsNull();
    }

    /// <summary>
    /// Validates that ViewContractObservable property is not null.
    /// </summary>
    [Test]
    public async Task ViewContractObservable_IsNotNull()
    {
        var host = new ViewModelViewHost();
        await Assert.That(host.ViewContractObservable).IsNotNull();
    }

    /// <summary>
    /// Validates that ViewContractObservable can be set.
    /// </summary>
    [Test]
    public async Task ViewContractObservable_CanBeSet()
    {
        var host = new ViewModelViewHost();
        var observable = RxObservable.Return("NewContract");
        host.ViewContractObservable = observable;
        await Assert.That(host.ViewContractObservable).IsEqualTo(observable);
    }

    /// <summary>
    /// Validates that setting ViewContract updates ViewContractObservable.
    /// </summary>
    [Test]
    public async Task ViewContract_Setter_UpdatesViewContractObservable()
    {
        var host = new ViewModelViewHost();
        var originalObservable = host.ViewContractObservable;
        host.ViewContract = "UpdatedContract";
        await Assert.That(host.ViewContractObservable).IsNotSameReferenceAs(originalObservable);
    }

    /// <summary>
    /// Validates that ViewModelProperty DependencyProperty exists.
    /// </summary>
    [Test]
    public async Task ViewModelProperty_DependencyProperty_Exists()
    {
        await Assert.That(ViewModelViewHost.ViewModelProperty).IsNotNull();
    }

    /// <summary>
    /// Validates that DefaultContentProperty DependencyProperty exists.
    /// </summary>
    [Test]
    public async Task DefaultContentProperty_DependencyProperty_Exists()
    {
        await Assert.That(ViewModelViewHost.DefaultContentProperty).IsNotNull();
    }

    /// <summary>
    /// Validates that ViewContractObservableProperty DependencyProperty exists.
    /// </summary>
    [Test]
    public async Task ViewContractObservableProperty_DependencyProperty_Exists()
    {
        await Assert.That(ViewModelViewHost.ViewContractObservableProperty).IsNotNull();
    }

    /// <summary>
    /// Validates that ViewModel can be set to null.
    /// </summary>
    [Test]
    public async Task ViewModel_CanBeSetToNull()
    {
        var host = new ViewModelViewHost();
        host.ViewModel = new TestViewModel();
        host.ViewModel = null;
        await Assert.That(host.ViewModel).IsNull();
    }

    /// <summary>
    /// Test view model for testing purposes.
    /// </summary>
    private sealed class TestViewModel
    {
    }
}

// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using TUnit.Assertions.Extensions;
using TUnit.Core;
using RxObservable = System.Reactive.Linq.Observable;

namespace ReactiveUI.Uno.Tests.Controls;

/// <summary>
/// Contains tests for the <see cref="RoutedViewHost"/> class.
/// </summary>
public class RoutedViewHostTests
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
    /// Validates that RoutedViewHost can be instantiated.
    /// </summary>
    [Test]
    public async Task RoutedViewHost_CanBeInstantiated()
    {
        var host = new RoutedViewHost();
        await Assert.That(host).IsNotNull();
    }

    /// <summary>
    /// Validates that RoutedViewHost implements IActivatableView.
    /// </summary>
    [Test]
    public async Task RoutedViewHost_ImplementsIActivatableView()
    {
        var host = new RoutedViewHost();
        await Assert.That(host).IsAssignableTo<IActivatableView>();
    }

    /// <summary>
    /// Validates that RoutedViewHost inherits from TransitioningContentControl.
    /// </summary>
    [Test]
    public async Task RoutedViewHost_InheritsFromTransitioningContentControl()
    {
        var host = new RoutedViewHost();
        await Assert.That(host).IsAssignableTo<TransitioningContentControl>();
    }

    /// <summary>
    /// Validates that Router property is initially null.
    /// </summary>
    [Test]
    public async Task Router_IsInitiallyNull()
    {
        var host = new RoutedViewHost();
        await Assert.That(host.Router).IsNull();
    }

    /// <summary>
    /// Validates that Router property can be set.
    /// </summary>
    [Test]
    public async Task Router_CanBeSet()
    {
        var host = new RoutedViewHost();
        var router = new RoutingState();
        host.Router = router;
        await Assert.That(host.Router).IsEqualTo(router);
    }

    /// <summary>
    /// Validates that DefaultContent property is initially null.
    /// </summary>
    [Test]
    public async Task DefaultContent_IsInitiallyNull()
    {
        var host = new RoutedViewHost();
        await Assert.That(host.DefaultContent).IsNull();
    }

    /// <summary>
    /// Validates that DefaultContent property can be set.
    /// </summary>
    [Test]
    public async Task DefaultContent_CanBeSet()
    {
        var host = new RoutedViewHost();
        var content = new object();
        host.DefaultContent = content;
        await Assert.That(host.DefaultContent).IsEqualTo(content);
    }

    /// <summary>
    /// Validates that ViewContract property can be set.
    /// </summary>
    [Test]
    public async Task ViewContract_CanBeSet()
    {
        var host = new RoutedViewHost();
        host.ViewContract = "TestContract";
        await Assert.That(host.ViewContract).IsEqualTo("TestContract");
    }

    /// <summary>
    /// Validates that ViewLocator property is initially null.
    /// </summary>
    [Test]
    public async Task ViewLocator_IsInitiallyNull()
    {
        var host = new RoutedViewHost();
        await Assert.That(host.ViewLocator).IsNull();
    }

    /// <summary>
    /// Validates that ViewContractObservable property is not null.
    /// </summary>
    [Test]
    public async Task ViewContractObservable_IsNotNull()
    {
        var host = new RoutedViewHost();
        await Assert.That(host.ViewContractObservable).IsNotNull();
    }

    /// <summary>
    /// Validates that ViewContractObservable can be set.
    /// </summary>
    [Test]
    public async Task ViewContractObservable_CanBeSet()
    {
        var host = new RoutedViewHost();
        var observable = RxObservable.Return("NewContract");
        host.ViewContractObservable = observable;
        await Assert.That(host.ViewContractObservable).IsEqualTo(observable);
    }

    /// <summary>
    /// Validates that HorizontalContentAlignment is set to Stretch by default.
    /// </summary>
    [Test]
    public async Task HorizontalContentAlignment_IsStretchByDefault()
    {
        var host = new RoutedViewHost();
        await Assert.That(host.HorizontalContentAlignment).IsEqualTo(HorizontalAlignment.Stretch);
    }

    /// <summary>
    /// Validates that VerticalContentAlignment is set to Stretch by default.
    /// </summary>
    [Test]
    public async Task VerticalContentAlignment_IsStretchByDefault()
    {
        var host = new RoutedViewHost();
        await Assert.That(host.VerticalContentAlignment).IsEqualTo(VerticalAlignment.Stretch);
    }

    /// <summary>
    /// Validates that setting ViewContract updates ViewContractObservable.
    /// </summary>
    [Test]
    public async Task ViewContract_Setter_UpdatesViewContractObservable()
    {
        var host = new RoutedViewHost();
        var originalObservable = host.ViewContractObservable;
        host.ViewContract = "UpdatedContract";
        await Assert.That(host.ViewContractObservable).IsNotSameReferenceAs(originalObservable);
    }
}

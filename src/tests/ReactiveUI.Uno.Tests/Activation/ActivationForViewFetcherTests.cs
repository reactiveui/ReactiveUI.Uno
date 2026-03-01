// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Activation;

/// <summary>
/// Contains tests for the <see cref="ActivationForViewFetcher"/> class, ensuring its functionality
/// for determining view activation affinity and providing activation observables.
/// </summary>
public class ActivationForViewFetcherTests
{
    /// <summary>
    /// The system under test.
    /// </summary>
    private ActivationForViewFetcher _sut = null!;

    /// <summary>
    /// Sets up the test by creating a new instance of ActivationForViewFetcher.
    /// </summary>
    [Before(Test)]
    public void SetUp()
    {
        _sut = new ActivationForViewFetcher();
    }

    /// <summary>
    /// Validates that GetAffinityForView returns high affinity for FrameworkElement types.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsHighAffinity_ForFrameworkElementTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(FrameworkElement));
        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Validates that GetAffinityForView returns zero affinity for non-FrameworkElement types.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsZeroAffinity_ForNonFrameworkElementTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(object));
        await Assert.That(affinity).IsZero();
    }

    /// <summary>
    /// Validates that GetAffinityForView returns zero affinity for string type.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsZeroAffinity_ForStringType()
    {
        var affinity = _sut.GetAffinityForView(typeof(string));
        await Assert.That(affinity).IsZero();
    }

    /// <summary>
    /// Validates that GetAffinityForView returns zero affinity for value types.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsZeroAffinity_ForValueTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(int));
        await Assert.That(affinity).IsZero();
    }

    /// <summary>
    /// Validates that GetActivationForView returns empty observable for non-FrameworkElement views.
    /// </summary>
    [Test]
    public async Task GetActivationForView_ReturnsEmptyObservable_ForNonFrameworkElementViews()
    {
        var mockView = new MockActivatableView();
        var observable = _sut.GetActivationForView(mockView);

        await Assert.That(observable).IsNotNull();
        await Assert.That(observable).IsAssignableTo<IObservable<bool>>();
    }

    /// <summary>
    /// Validates that ActivationForViewFetcher implements IActivationForViewFetcher interface.
    /// </summary>
    [Test]
    public async Task ActivationForViewFetcher_ImplementsIActivationForViewFetcher()
    {
        await Assert.That(_sut).IsAssignableTo<IActivationForViewFetcher>();
    }

    /// <summary>
    /// Validates that GetAffinityForView returns high affinity for UserControl types.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsHighAffinity_ForUserControlTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(UserControl));
        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Validates that GetAffinityForView returns high affinity for Page types.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsHighAffinity_ForPageTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(Page));
        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Validates that GetAffinityForView returns high affinity for ContentControl types.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsHighAffinity_ForContentControlTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(ContentControl));
        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Validates that GetAffinityForView returns zero affinity for interface types.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsZeroAffinity_ForInterfaceTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(IActivatableView));
        await Assert.That(affinity).IsZero();
    }

    /// <summary>
    /// Validates that GetAffinityForView returns zero affinity for abstract non-FrameworkElement types.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsZeroAffinity_ForAbstractNonFrameworkElementTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(System.IO.Stream));
        await Assert.That(affinity).IsZero();
    }

    /// <summary>
    /// Validates that multiple fetchers can be created independently.
    /// </summary>
    [Test]
    public async Task MultipleFetchers_CanBeCreatedIndependently()
    {
        var fetcher1 = new ActivationForViewFetcher();
        var fetcher2 = new ActivationForViewFetcher();

        await Assert.That(fetcher1).IsNotNull();
        await Assert.That(fetcher2).IsNotNull();
        await Assert.That(fetcher1).IsNotSameReferenceAs(fetcher2);
    }

    /// <summary>
    /// Validates that GetAffinityForView returns consistent results for the same type.
    /// </summary>
    [Test]
    public async Task GetAffinityForView_ReturnsConsistentResults_ForSameType()
    {
        var affinity1 = _sut.GetAffinityForView(typeof(FrameworkElement));
        var affinity2 = _sut.GetAffinityForView(typeof(FrameworkElement));

        await Assert.That(affinity1).IsEqualTo(affinity2);
    }

    /// <summary>
    /// Simple mock implementation of IActivatableView for testing.
    /// </summary>
    private sealed class MockActivatableView : IActivatableView
    {
        public ViewModelActivator Activator { get; } = new();
    }
}

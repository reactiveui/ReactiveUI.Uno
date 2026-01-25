// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
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
    /// Simple mock implementation of IActivatableView for testing.
    /// </summary>
    private class MockActivatableView : IActivatableView
    {
        public ViewModelActivator Activator { get; } = new();
    }
}

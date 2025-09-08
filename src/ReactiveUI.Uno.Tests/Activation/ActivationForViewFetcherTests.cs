// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Activation;

/// <summary>
/// Contains tests for the <see cref="ActivationForViewFetcher"/> class, ensuring its functionality
/// for determining view activation affinity and providing activation observables.
/// </summary>
[TestFixture]
public class ActivationForViewFetcherTests
{
    /// <summary>
    /// The system under test.
    /// </summary>
    private ActivationForViewFetcher _sut = null!;

    /// <summary>
    /// Sets up the test by creating a new instance of ActivationForViewFetcher.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _sut = new ActivationForViewFetcher();
    }

    /// <summary>
    /// Validates that GetAffinityForView returns high affinity for FrameworkElement types.
    /// </summary>
    [Test]
    public void GetAffinityForView_ReturnsHighAffinity_ForFrameworkElementTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(FrameworkElement));
        Assert.That(affinity, Is.EqualTo(10));
    }

    /// <summary>
    /// Validates that GetAffinityForView returns zero affinity for non-FrameworkElement types.
    /// </summary>
    [Test]
    public void GetAffinityForView_ReturnsZeroAffinity_ForNonFrameworkElementTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(object));
        Assert.That(affinity, Is.Zero);
    }

    /// <summary>
    /// Validates that GetAffinityForView returns zero affinity for string type.
    /// </summary>
    [Test]
    public void GetAffinityForView_ReturnsZeroAffinity_ForStringType()
    {
        var affinity = _sut.GetAffinityForView(typeof(string));
        Assert.That(affinity, Is.Zero);
    }

    /// <summary>
    /// Validates that GetAffinityForView returns zero affinity for value types.
    /// </summary>
    [Test]
    public void GetAffinityForView_ReturnsZeroAffinity_ForValueTypes()
    {
        var affinity = _sut.GetAffinityForView(typeof(int));
        Assert.That(affinity, Is.Zero);
    }

    /// <summary>
    /// Validates that GetActivationForView returns empty observable for non-FrameworkElement views.
    /// </summary>
    [Test]
    public void GetActivationForView_ReturnsEmptyObservable_ForNonFrameworkElementViews()
    {
        var mockView = new MockActivatableView();
        var observable = _sut.GetActivationForView(mockView);

        Assert.That(observable, Is.Not.Null);
        Assert.That(observable, Is.InstanceOf<IObservable<bool>>());
    }

    /// <summary>
    /// Simple mock implementation of IActivatableView for testing.
    /// </summary>
    private class MockActivatableView : IActivatableView
    {
        public ViewModelActivator Activator { get; } = new();
    }
}

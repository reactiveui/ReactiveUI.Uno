// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Linq;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>
/// Contains tests for the <see cref="WinRTAppDataDriver"/> class, ensuring its functionality
/// for loading, saving, and invalidating application state.
/// </summary>
[TestFixture]
public class WinRTAppDataDriverTests
{
    /// <summary>
    /// The system under test.
    /// </summary>
    private WinRTAppDataDriver _sut = null!;

    /// <summary>
    /// Sets up the test by creating a new instance of WinRTAppDataDriver.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        _sut = new WinRTAppDataDriver();
    }

    /// <summary>
    /// Validates that SaveState throws ArgumentNullException when state is null.
    /// </summary>
    [Test]
    public void SaveState_ThrowsArgumentNullException_WhenStateIsNull()
    {
        Assert.That(() => _sut.SaveState(null!), Throws.ArgumentNullException);
    }

    /// <summary>
    /// Validates that SaveState returns an observable that can be subscribed to.
    /// </summary>
    [Test]
    public void SaveState_ReturnsObservable_WhenStateIsValid()
    {
        var state = new { Name = "Test", Value = 42 };
        var observable = _sut.SaveState(state);

        Assert.That(observable, Is.Not.Null);
        Assert.That(observable, Is.InstanceOf<IObservable<System.Reactive.Unit>>());
    }

    /// <summary>
    /// Validates that LoadState returns an observable.
    /// </summary>
    [Test]
    public void LoadState_ReturnsObservable()
    {
        var observable = _sut.LoadState();

        Assert.That(observable, Is.Not.Null);
        Assert.That(observable, Is.InstanceOf<IObservable<object>>());
    }

    /// <summary>
    /// Validates that InvalidateState returns an observable.
    /// </summary>
    [Test]
    public void InvalidateState_ReturnsObservable()
    {
        var observable = _sut.InvalidateState();

        Assert.That(observable, Is.Not.Null);
        Assert.That(observable, Is.InstanceOf<IObservable<System.Reactive.Unit>>());
    }
}

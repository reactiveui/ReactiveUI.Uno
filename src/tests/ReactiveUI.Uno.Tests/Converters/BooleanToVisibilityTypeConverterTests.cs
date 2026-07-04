// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
#if REACTIVE_SHIM
using ReactiveUI.Uno.Reactive;
#else
using ReactiveUI.Uno;
#endif
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Converters;

/// <summary>
/// Unit tests for the BooleanToVisibilityTypeConverter class, which provides conversions between boolean values and Microsoft.UI.Xaml.Visibility.
/// </summary>
public class BooleanToVisibilityTypeConverterTests
{
    /// <summary>Represents the system under test, an instance of the <see cref="BooleanToVisibilityTypeConverter"/> class.</summary>
    private readonly BooleanToVisibilityTypeConverter _sut = new();

    /// <summary>Determines the affinity level for conversions.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObjects_ReturnsExpectedValue()
    {
        await Assert.That(_sut.GetAffinityForObjects()).IsEqualTo(2);
    }

    /// <summary>Converts a boolean true value to Visibility.Visible.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_TrueToVisibility_ReturnsVisible()
    {
        var success = _sut.TryConvert(true, BooleanToVisibilityHint.None, out var result);
        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }

    /// <summary>Converts a boolean false value to Visibility.Collapsed.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_FalseToVisibility_ReturnsCollapsed()
    {
        var success = _sut.TryConvert(false, BooleanToVisibilityHint.None, out var result);
        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

    /// <summary>Converts a boolean true value with Inverse hint to Visibility.Collapsed.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_TrueWithInverse_ReturnsCollapsed()
    {
        var success = _sut.TryConvert(true, BooleanToVisibilityHint.Inverse, out var result);
        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Collapsed);
    }

    /// <summary>Converts a boolean false value with Inverse hint to Visibility.Visible.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_FalseWithInverse_ReturnsVisible()
    {
        var success = _sut.TryConvert(false, BooleanToVisibilityHint.Inverse, out var result);
        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }

    /// <summary>Validates conversion with null conversionHint defaults to None.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_WithNullHint_DefaultsToNone()
    {
        var success = _sut.TryConvert(true, null, out var result);
        await Assert.That(success).IsTrue();
        await Assert.That(result).IsEqualTo(Visibility.Visible);
    }

    /// <summary>Validates that BooleanToVisibilityTypeConverter implements IBindingTypeConverter.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task BooleanToVisibilityTypeConverter_ImplementsIBindingTypeConverter()
    {
        await Assert.That(_sut).IsAssignableTo<IBindingTypeConverter>();
    }

    /// <summary>Validates that multiple conversions return consistent results.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task TryConvert_MultipleConversions_ReturnsConsistentResults()
    {
        _ = _sut.TryConvert(true, BooleanToVisibilityHint.None, out var result1);
        _ = _sut.TryConvert(true, BooleanToVisibilityHint.None, out var result2);

        await Assert.That(result1).IsEqualTo(result2);
    }
}

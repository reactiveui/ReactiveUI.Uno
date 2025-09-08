// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Converters;

/// <summary>
/// Unit tests for the BooleanToVisibilityTypeConverter class, which provides conversions and inverse conversions between boolean values and Microsoft.UI.Xaml.Visibility.
/// </summary>
[TestFixture]
public class BooleanToVisibilityTypeConverterTests
{
    /// <summary>
    /// Represents the system under test, an instance of the <see cref="BooleanToVisibilityTypeConverter"/> class.
    /// This instance is used to validate the logic of converting between <see cref="bool"/> and <see cref="Microsoft.UI.Xaml.Visibility"/> values,
    /// including its support for both normal and inverse conversion modes.
    /// </summary>
    private readonly BooleanToVisibilityTypeConverter _sut = new();

    /// <summary>
    /// Determines the affinity level for conversions between boolean values and Microsoft.UI.Xaml.Visibility.
    /// </summary>
    [Test]
    public void Affinity_is_high_for_bool_to_visibility()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_sut.GetAffinityForObjects(typeof(bool), typeof(Visibility)), Is.EqualTo(10));
            Assert.That(_sut.GetAffinityForObjects(typeof(Visibility), typeof(bool)), Is.EqualTo(10));
        }
    }

    /// <summary>
    /// Converts a boolean value to the Microsoft.UI.Xaml.Visibility enumeration.
    /// </summary>
    /// <param name="input">The boolean input to convert.</param>
    /// <param name="expected">The expected Microsoft.UI.Xaml.Visibility output after conversion.</param>
    [TestCase(true, Visibility.Visible)]
    [TestCase(false, Visibility.Collapsed)]
    public void Converts_bool_to_visibility(bool input, Visibility expected)
    {
        var success = _sut.TryConvert(input, typeof(Visibility), BooleanToVisibilityHint.None, out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// Convertses the bool to visibility inverse.
    /// </summary>
    /// <param name="input">if set to <c>true</c> [input].</param>
    /// <param name="expected">The expected.</param>
    [TestCase(true, Visibility.Collapsed)]
    [TestCase(false, Visibility.Visible)]
    public void Converts_bool_to_visibility_inverse(bool input, Visibility expected)
    {
        var success = _sut.TryConvert(input, typeof(Visibility), BooleanToVisibilityHint.Inverse, out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// Convertses the visibility to bool.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="expected">if set to <c>true</c> [expected].</param>
    [TestCase(Visibility.Visible, true)]
    [TestCase(Visibility.Collapsed, false)]
    public void Converts_visibility_to_bool(Visibility input, bool expected)
    {
        var success = _sut.TryConvert(input, typeof(bool), BooleanToVisibilityHint.None, out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// Convertses the visibility to bool inverse.
    /// </summary>
    /// <param name="input">The input.</param>
    /// <param name="expected">if set to <c>true</c> [expected].</param>
    [TestCase(Visibility.Visible, false)]
    [TestCase(Visibility.Collapsed, true)]
    public void Converts_visibility_to_bool_inverse(Visibility input, bool expected)
    {
        var success = _sut.TryConvert(input, typeof(bool), BooleanToVisibilityHint.Inverse, out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(expected));
        }
    }

    /// <summary>
    /// Validates that GetAffinityForObjects returns zero for unsupported type combinations.
    /// </summary>
    [Test]
    public void GetAffinityForObjects_ReturnsZero_ForUnsupportedTypes()
    {
        using (Assert.EnterMultipleScope())
        {
            Assert.That(_sut.GetAffinityForObjects(typeof(string), typeof(bool)), Is.Zero);
            Assert.That(_sut.GetAffinityForObjects(typeof(bool), typeof(string)), Is.Zero);
            Assert.That(_sut.GetAffinityForObjects(typeof(int), typeof(Visibility)), Is.Zero);
            Assert.That(_sut.GetAffinityForObjects(typeof(object), typeof(object)), Is.Zero);
        }
    }

    /// <summary>
    /// Validates conversion with null conversionHint defaults to None.
    /// </summary>
    [Test]
    public void TryConvert_WithNullHint_DefaultsToNone()
    {
        var success = _sut.TryConvert(true, typeof(Visibility), null, out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }
    }

    /// <summary>
    /// Validates fallback behavior for unsupported input types.
    /// </summary>
    [Test]
    public void TryConvert_WithUnsupportedInput_ProvidesFallback()
    {
        var success = _sut.TryConvert("invalid", typeof(Visibility), BooleanToVisibilityHint.None, out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }
    }

    /// <summary>
    /// Validates fallback behavior when converting to bool with unsupported input.
    /// </summary>
    [Test]
    public void TryConvert_WithUnsupportedInputToBool_ProvidesFallback()
    {
        var success = _sut.TryConvert("invalid", typeof(bool), BooleanToVisibilityHint.None, out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.False);
        }
    }

    /// <summary>
    /// Validates conversion with non-BooleanToVisibilityHint conversionHint defaults to None.
    /// </summary>
    [Test]
    public void TryConvert_WithNonBooleanToVisibilityHint_DefaultsToNone()
    {
        var success = _sut.TryConvert(true, typeof(Visibility), "string hint", out var result);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(success, Is.True);
            Assert.That(result, Is.EqualTo(Visibility.Visible));
        }
    }
}

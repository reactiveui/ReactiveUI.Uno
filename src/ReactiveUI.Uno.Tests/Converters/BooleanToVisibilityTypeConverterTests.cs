// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FluentAssertions;
using Microsoft.UI.Xaml;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Converters;

/// <summary>
/// BooleanToVisibilityTypeConverterTests.
/// </summary>
[TestFixture]
public class BooleanToVisibilityTypeConverterTests
{
    private readonly BooleanToVisibilityTypeConverter _sut = new();

    /// <summary>
    /// Affinities the is high for bool to visibility.
    /// </summary>
    [Test]
    public void Affinity_is_high_for_bool_to_visibility()
    {
        _sut.GetAffinityForObjects(typeof(bool), typeof(Visibility)).Should().Be(10);
        _sut.GetAffinityForObjects(typeof(Visibility), typeof(bool)).Should().Be(10);
    }

    /// <summary>
    /// Convertses the bool to visibility.
    /// </summary>
    /// <param name="input">if set to <c>true</c> [input].</param>
    /// <param name="expected">The expected.</param>
    [TestCase(true, Visibility.Visible)]
    [TestCase(false, Visibility.Collapsed)]
    public void Converts_bool_to_visibility(bool input, Visibility expected)
    {
        _sut.TryConvert(input, typeof(Visibility), BooleanToVisibilityHint.None, out var result).Should().BeTrue();
        result.Should().Be(expected);
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
        _sut.TryConvert(input, typeof(Visibility), BooleanToVisibilityHint.Inverse, out var result).Should().BeTrue();
        result.Should().Be(expected);
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
        _sut.TryConvert(input, typeof(bool), BooleanToVisibilityHint.None, out var result).Should().BeTrue();
        result.Should().Be(expected);
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
        _sut.TryConvert(input, typeof(bool), BooleanToVisibilityHint.Inverse, out var result).Should().BeTrue();
        result.Should().Be(expected);
    }
}

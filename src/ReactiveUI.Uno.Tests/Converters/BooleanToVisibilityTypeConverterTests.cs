// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FluentAssertions;
using Xunit;
#if HAS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace ReactiveUI.Uno.Tests.Converters;

public class BooleanToVisibilityTypeConverterTests
{
    private readonly BooleanToVisibilityTypeConverter _sut = new();

    [Fact]
    public void Affinity_is_high_for_bool_to_visibility()
    {
        _sut.GetAffinityForObjects(typeof(bool), typeof(Visibility)).Should().Be(10);
        _sut.GetAffinityForObjects(typeof(Visibility), typeof(bool)).Should().Be(10);
    }

    [Theory]
    [InlineData(true, Visibility.Visible)]
    [InlineData(false, Visibility.Collapsed)]
    public void Converts_bool_to_visibility(bool input, Visibility expected)
    {
        _sut.TryConvert(input, typeof(Visibility), BooleanToVisibilityHint.None, out var result).Should().BeTrue();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, Visibility.Collapsed)]
    [InlineData(false, Visibility.Visible)]
    public void Converts_bool_to_visibility_inverse(bool input, Visibility expected)
    {
        _sut.TryConvert(input, typeof(Visibility), BooleanToVisibilityHint.Inverse, out var result).Should().BeTrue();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(Visibility.Visible, true)]
    [InlineData(Visibility.Collapsed, false)]
    public void Converts_visibility_to_bool(Visibility input, bool expected)
    {
        _sut.TryConvert(input, typeof(bool), BooleanToVisibilityHint.None, out var result).Should().BeTrue();
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(Visibility.Visible, false)]
    [InlineData(Visibility.Collapsed, true)]
    public void Converts_visibility_to_bool_inverse(Visibility input, bool expected)
    {
        _sut.TryConvert(input, typeof(bool), BooleanToVisibilityHint.Inverse, out var result).Should().BeTrue();
        result.Should().Be(expected);
    }
}

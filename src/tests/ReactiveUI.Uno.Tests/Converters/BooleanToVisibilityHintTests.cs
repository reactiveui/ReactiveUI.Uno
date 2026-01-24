// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Converters;

/// <summary>
/// Tests for the <see cref="BooleanToVisibilityHint"/> enum.
/// </summary>
public class BooleanToVisibilityHintTests
{
    /// <summary>
    /// Verifies None has correct value.
    /// </summary>
    [Test]
    public async Task None_HasCorrectValue()
    {
        // Arrange
        var noneValue = (int)BooleanToVisibilityHint.None;

        // Assert
        await Assert.That(noneValue).IsZero();
    }

    /// <summary>
    /// Verifies Inverse has correct value and string representation.
    /// </summary>
    [Test]
    public async Task Inverse_HasCorrectValueAndString()
    {
        // Arrange
        var inverseValue = (int)BooleanToVisibilityHint.Inverse;

        // Assert
        await Assert.That(inverseValue).IsEqualTo(2);
    }

    /// <summary>
    /// Verifies Inverse has correct string representation.
    /// </summary>
    [Test]
    public async Task Inverse_HasCorrectString()
    {
        // Assert
        await Assert.That(BooleanToVisibilityHint.Inverse.ToString()).IsEqualTo("Inverse");
    }

    /// <summary>
    /// Verifies enum supports flags operation.
    /// </summary>
    [Test]
    public async Task Enum_SupportsFlags()
    {
        // Act
        var combined = BooleanToVisibilityHint.None | BooleanToVisibilityHint.Inverse;

        // Assert
        await Assert.That(combined).IsEqualTo(BooleanToVisibilityHint.Inverse);
    }

    /// <summary>
    /// Verifies enum has Flags attribute.
    /// </summary>
    [Test]
    public async Task Enum_HasFlagsAttribute()
    {
        // Arrange
        var enumType = typeof(BooleanToVisibilityHint);

        // Act
        var hasFlagsAttribute = enumType.IsDefined(typeof(FlagsAttribute), false);

        // Assert
        await Assert.That(hasFlagsAttribute).IsTrue();
    }

    /// <summary>
    /// Verifies enum can be cast from integer.
    /// </summary>
    [Test]
    public async Task Enum_CanBeCastFromInteger()
    {
        // Act
        var none = (BooleanToVisibilityHint)0;

        // Assert
        await Assert.That(none).IsEqualTo(BooleanToVisibilityHint.None);
    }

    /// <summary>
    /// Verifies enum can be cast from integer for inverse value.
    /// </summary>
    [Test]
    public async Task Enum_CanBeCastFromInteger_Inverse()
    {
        // Act
        var inverse = (BooleanToVisibilityHint)2;

        // Assert
        await Assert.That(inverse).IsEqualTo(BooleanToVisibilityHint.Inverse);
    }
}

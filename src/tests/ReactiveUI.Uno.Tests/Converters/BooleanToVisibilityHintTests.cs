// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Converters;

/// <summary>Tests for the <see cref="BooleanToVisibilityHint"/> enum.</summary>
public class BooleanToVisibilityHintTests
{
    /// <summary>Verifies None has correct value.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task None_HasCorrectValue()
    {
        // Arrange
        var noneValue = Convert.ToInt32(BooleanToVisibilityHint.None);

        // Assert
        await Assert.That(noneValue).IsZero();
    }

    /// <summary>Verifies Inverse has correct value and string representation.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Inverse_HasCorrectValueAndString()
    {
        // Arrange
        var inverseValue = Convert.ToInt32(BooleanToVisibilityHint.Inverse);

        // Assert
        await Assert.That(inverseValue).IsEqualTo(2);
    }

    /// <summary>Verifies Inverse has correct string representation.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Inverse_HasCorrectString()
    {
        // Assert
        await Assert.That(BooleanToVisibilityHint.Inverse.ToString()).IsEqualTo("Inverse");
    }

    /// <summary>Verifies enum supports flags operation.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Enum_SupportsFlags()
    {
        // Act
        var initial = Enum.Parse<BooleanToVisibilityHint>(nameof(BooleanToVisibilityHint.None));
        var combined = initial | BooleanToVisibilityHint.Inverse;

        // Assert
        await Assert.That(combined).IsEqualTo(BooleanToVisibilityHint.Inverse);
    }

    /// <summary>Verifies enum has Flags attribute.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
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

    /// <summary>Verifies enum can be cast from integer.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Enum_CanBeCastFromInteger()
    {
        // Act
        var none = (BooleanToVisibilityHint)Enum.ToObject(typeof(BooleanToVisibilityHint), 0);

        // Assert
        await Assert.That(none).IsEqualTo(BooleanToVisibilityHint.None);
    }

    /// <summary>Verifies enum can be cast from integer for inverse value.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Enum_CanBeCastFromInteger_Inverse()
    {
        // Act
        var inverse = (BooleanToVisibilityHint)Enum.ToObject(typeof(BooleanToVisibilityHint), 2);

        // Assert
        await Assert.That(inverse).IsEqualTo(BooleanToVisibilityHint.Inverse);
    }
}

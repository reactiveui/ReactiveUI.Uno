// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Converters;

/// <summary>
/// Tests for the <see cref="BooleanToVisibilityHint"/> enum.
/// </summary>
[TestFixture]
public class BooleanToVisibilityHintTests
{
    /// <summary>
    /// Verifies None has correct value.
    /// </summary>
    [Test]
    public void None_HasCorrectValue()
    {
        // Assert
        Assert.That((int)BooleanToVisibilityHint.None, Is.Zero);
    }

    /// <summary>
    /// Verifies Inverse has correct value and string representation.
    /// </summary>
    [Test]
    public void Inverse_HasCorrectValueAndString()
    {
        // Assert
        Assert.That((int)BooleanToVisibilityHint.Inverse, Is.EqualTo(2));
    }

    /// <summary>
    /// Verifies Inverse has correct string representation.
    /// </summary>
    [Test]
    public void Inverse_HasCorrectString()
    {
        // Assert
        Assert.That(BooleanToVisibilityHint.Inverse.ToString(), Is.EqualTo("Inverse"));
    }

    /// <summary>
    /// Verifies enum supports flags operation.
    /// </summary>
    [Test]
    public void Enum_SupportsFlags()
    {
        // Act
        var combined = BooleanToVisibilityHint.None | BooleanToVisibilityHint.Inverse;

        // Assert
        Assert.That(combined, Is.EqualTo(BooleanToVisibilityHint.Inverse));
    }

    /// <summary>
    /// Verifies enum has Flags attribute.
    /// </summary>
    [Test]
    public void Enum_HasFlagsAttribute()
    {
        // Arrange
        var enumType = typeof(BooleanToVisibilityHint);

        // Act
        var hasFlagsAttribute = enumType.IsDefined(typeof(FlagsAttribute), false);

        // Assert
        Assert.That(hasFlagsAttribute, Is.True);
    }

    /// <summary>
    /// Verifies enum can be cast from integer.
    /// </summary>
    [Test]
    public void Enum_CanBeCastFromInteger()
    {
        // Act
        var none = (BooleanToVisibilityHint)0;

        // Assert
        Assert.That(none, Is.EqualTo(BooleanToVisibilityHint.None));
    }

    /// <summary>
    /// Verifies enum can be cast from integer for inverse value.
    /// </summary>
    [Test]
    public void Enum_CanBeCastFromInteger_Inverse()
    {
        // Act
        var inverse = (BooleanToVisibilityHint)2;

        // Assert
        Assert.That(inverse, Is.EqualTo(BooleanToVisibilityHint.Inverse));
    }
}

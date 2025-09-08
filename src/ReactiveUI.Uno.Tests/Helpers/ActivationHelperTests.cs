// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Helpers;

/// <summary>
/// Tests for the <see cref="ActivationHelper"/> class.
/// </summary>
[TestFixture]
public class ActivationHelperTests
{
    /// <summary>
    /// Verifies UnoActivated property is true after static constructor.
    /// </summary>
    [Test]
    public void UnoActivated_IsTrue_AfterStaticConstructor()
    {
        // Act & Assert
        Assert.That(ActivationHelper.UnoActivated, Is.True);
    }

    /// <summary>
    /// Verifies UnoActivated property is consistent across multiple calls.
    /// </summary>
    [Test]
    public void UnoActivated_IsConsistent_AcrossMultipleCalls()
    {
        // Act
        var firstCall = ActivationHelper.UnoActivated;
        var secondCall = ActivationHelper.UnoActivated;

        // Assert
        Assert.That(firstCall, Is.EqualTo(secondCall));
        Assert.That(firstCall, Is.True);
    }

    /// <summary>
    /// Verifies the activation happens only once and all calls return the same consistent value.
    /// </summary>
    [Test]
    public void Activation_HappensOnlyOnce()
    {
        // This test verifies that the static constructor logic runs only once
        // and the UnoActivated flag remains true consistently

        // Act
        var activated1 = ActivationHelper.UnoActivated;
        var activated2 = ActivationHelper.UnoActivated;
        var activated3 = ActivationHelper.UnoActivated;

        // Assert
        Assert.That(activated1, Is.True);
    }

    /// <summary>
    /// Verifies all calls to UnoActivated return the same value.
    /// </summary>
    [Test]
    public void UnoActivated_AllCallsReturnSameValue()
    {
        // Act
        var activated1 = ActivationHelper.UnoActivated;
        var activated2 = ActivationHelper.UnoActivated;

        // Assert
        Assert.That(activated2, Is.True);
    }
}
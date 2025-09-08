// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Platform;

/// <summary>
/// Contains test cases to ensure the functionality and correctness of the <see cref="PlatformOperations"/> class.
/// </summary>
[TestFixture]
public class PlatformOperationsTests
{
    /// <summary>
    /// Validates that the GetOrientation method either returns a valid string representing the device orientation
    /// or null, and does not throw an exception during execution.
    /// </summary>
    [Test]
    public void GetOrientation_has_valid_string_or_null()
    {
        PlatformOperations ops = new();
        var orientation = ops.GetOrientation();

        // Accept null or any string value
        if (orientation is null)
        {
            return;
        }

        Assert.That(orientation, Is.InstanceOf<string>());
        Assert.That(orientation, Is.Not.Null);
    }

    /// <summary>
    /// Validates that PlatformOperations can be instantiated multiple times.
    /// </summary>
    [Test]
    public void Constructor_AllowsMultipleInstances()
    {
        // Act & Assert
        Assert.That(() => new PlatformOperations(), Throws.Nothing);
        Assert.That(() => new PlatformOperations(), Throws.Nothing);

        var ops1 = new PlatformOperations();
        var ops2 = new PlatformOperations();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(ops1, Is.Not.Null);
            Assert.That(ops2, Is.Not.Null);
        }

        Assert.That(ops1, Is.Not.SameAs(ops2));
    }

    /// <summary>
    /// Validates that GetOrientation is consistent when called multiple times.
    /// </summary>
    [Test]
    public void GetOrientation_IsConsistent()
    {
        var ops = new PlatformOperations();

        var orientation1 = ops.GetOrientation();
        var orientation2 = ops.GetOrientation();

        // Orientation should be consistent for the same instance
        Assert.That(orientation1, Is.EqualTo(orientation2));
    }

    /// <summary>
    /// Validates that PlatformOperations implements IPlatformOperations.
    /// </summary>
    [Test]
    public void PlatformOperations_ImplementsIPlatformOperations()
    {
        var ops = new PlatformOperations();

        Assert.That(ops, Is.InstanceOf<IPlatformOperations>());
    }

    /// <summary>
    /// Validates that GetOrientation doesn't throw under normal conditions.
    /// </summary>
    [Test]
    public void GetOrientation_DoesNotThrow()
    {
        var ops = new PlatformOperations();

        Assert.That(() => ops.GetOrientation(), Throws.Nothing);
    }

    /// <summary>
    /// Validates that orientation string (if not null) is non-empty.
    /// </summary>
    [Test]
    public void GetOrientation_IfNotNull_IsNotEmpty()
    {
        var ops = new PlatformOperations();
        var orientation = ops.GetOrientation();

        if (orientation is not null)
        {
            Assert.That(orientation, Is.Not.Empty);
        }
    }
}

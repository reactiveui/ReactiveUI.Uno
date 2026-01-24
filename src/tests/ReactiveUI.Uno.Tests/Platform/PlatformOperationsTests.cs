// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Platform;

/// <summary>
/// Contains test cases to ensure the functionality and correctness of the <see cref="PlatformOperations"/> class.
/// </summary>
public class PlatformOperationsTests
{
    /// <summary>
    /// Validates that the GetOrientation method either returns a valid string representing the device orientation
    /// or null, and does not throw an exception during execution.
    /// </summary>
    [Test]
    public async Task GetOrientation_has_valid_string_or_null()
    {
        PlatformOperations ops = new();
        var orientation = ops.GetOrientation();

        // Accept null or any string value
        if (orientation is null)
        {
            return;
        }

        await Assert.That(orientation).IsTypeOf<string>();
        await Assert.That(orientation).IsNotNull();
    }

    /// <summary>
    /// Validates that PlatformOperations can be instantiated multiple times.
    /// </summary>
    [Test]
    public async Task Constructor_AllowsMultipleInstances()
    {
        // Act & Assert
        var ops1 = new PlatformOperations();
        var ops2 = new PlatformOperations();

        await Assert.That(ops1).IsNotNull();
        await Assert.That(ops2).IsNotNull();
        await Assert.That(ops1).IsNotSameReferenceAs(ops2);
    }

    /// <summary>
    /// Validates that GetOrientation is consistent when called multiple times.
    /// </summary>
    [Test]
    public async Task GetOrientation_IsConsistent()
    {
        var ops = new PlatformOperations();

        var orientation1 = ops.GetOrientation();
        var orientation2 = ops.GetOrientation();

        // Orientation should be consistent for the same instance
        await Assert.That(orientation1).IsEqualTo(orientation2);
    }

    /// <summary>
    /// Validates that PlatformOperations implements IPlatformOperations.
    /// </summary>
    [Test]
    public async Task PlatformOperations_ImplementsIPlatformOperations()
    {
        var ops = new PlatformOperations();

        await Assert.That(ops).IsAssignableTo<IPlatformOperations>();
    }

    /// <summary>
    /// Validates that GetOrientation doesn't throw under normal conditions.
    /// </summary>
    [Test]
    public async Task GetOrientation_DoesNotThrow()
    {
        var ops = new PlatformOperations();

        await Assert.That(() => ops.GetOrientation()).ThrowsNothing();
    }

    /// <summary>
    /// Validates that orientation string (if not null) is non-empty.
    /// </summary>
    [Test]
    public async Task GetOrientation_IfNotNull_IsNotEmpty()
    {
        var ops = new PlatformOperations();
        var orientation = ops.GetOrientation();

        if (orientation is not null)
        {
            await Assert.That(orientation).IsNotEmpty();
        }
    }
}

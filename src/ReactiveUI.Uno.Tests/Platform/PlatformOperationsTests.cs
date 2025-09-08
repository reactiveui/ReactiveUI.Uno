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
}

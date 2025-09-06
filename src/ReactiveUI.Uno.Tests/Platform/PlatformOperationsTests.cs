// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FluentAssertions;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Platform;

/// <summary>
/// Tests for PlatformOperations.
/// </summary>
[TestFixture]
public class PlatformOperationsTests
{
    /// <summary>
    /// Ensures GetOrientation returns either null or a string value without throwing.
    /// </summary>
    [Test]
    public void GetOrientation_has_valid_string_or_null()
    {
        var ops = new ReactiveUI.Uno.PlatformOperations();
        var orientation = ops.GetOrientation();

        // Accept null or any string value
        (orientation == null || orientation.GetType() == typeof(string)).Should().BeTrue();
    }
}

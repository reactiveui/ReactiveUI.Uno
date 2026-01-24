// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Uno;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Helpers;

/// <summary>
/// Tests for activation helper functionality.
/// </summary>
public class ActivationHelperTests
{
    /// <summary>
    /// Verifies ActivationForViewFetcher can be instantiated.
    /// </summary>
    [Test]
    public async Task ActivationForViewFetcher_CanBeInstantiated()
    {
        // Act
        var fetcher = new ActivationForViewFetcher();

        // Assert
        await Assert.That(fetcher).IsNotNull();
    }

    /// <summary>
    /// Verifies ActivationForViewFetcher implements IActivationForViewFetcher.
    /// </summary>
    [Test]
    public async Task ActivationForViewFetcher_ImplementsInterface()
    {
        // Act
        var fetcher = new ActivationForViewFetcher();

        // Assert
        await Assert.That(fetcher).IsAssignableTo<IActivationForViewFetcher>();
    }
}

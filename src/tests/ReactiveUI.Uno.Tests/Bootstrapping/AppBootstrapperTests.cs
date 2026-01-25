// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Bootstrapping;

/// <summary>
/// Contains tests for the <see cref="AppBootstrapper"/> class, ensuring its functionality
/// for application bootstrapping with ReactiveUI.
/// </summary>
public class AppBootstrapperTests
{
    /// <summary>
    /// Validates that AppBootstrapper can be instantiated without errors.
    /// </summary>
    [Test]
    public async Task Constructor_CreatesInstance_Successfully()
    {
        AppBootstrapper? bootstrapper = null;

        bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsNotNull();
    }

    /// <summary>
    /// Validates that AppBootstrapper implements IScreen interface.
    /// </summary>
    [Test]
    public async Task AppBootstrapper_ImplementsIScreen()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsAssignableTo<IScreen>();
    }

    /// <summary>
    /// Validates that Router property is initialized and not null.
    /// </summary>
    [Test]
    public async Task Router_IsInitialized_NotNull()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper.Router).IsNotNull();
        await Assert.That(bootstrapper.Router).IsAssignableTo<RoutingState>();
    }

    /// <summary>
    /// Validates that AppBootstrapper inherits from ReactiveObject.
    /// </summary>
    [Test]
    public async Task AppBootstrapper_InheritsFromReactiveObject()
    {
        var bootstrapper = new AppBootstrapper();
        await Assert.That(bootstrapper).IsAssignableTo<ReactiveObject>();
    }

    /// <summary>
    /// Validates that multiple instances can be created.
    /// </summary>
    [Test]
    public async Task Constructor_AllowsMultipleInstances()
    {
        var bootstrapper1 = new AppBootstrapper();
        var bootstrapper2 = new AppBootstrapper();

        await Assert.That(bootstrapper1).IsNotNull();
        await Assert.That(bootstrapper2).IsNotNull();
        await Assert.That(bootstrapper1).IsNotSameReferenceAs(bootstrapper2);
        await Assert.That(bootstrapper1.Router).IsNotSameReferenceAs(bootstrapper2.Router);
    }
}

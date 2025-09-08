// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Bootstrapping;

/// <summary>
/// Contains tests for the <see cref="AppBootstrapper"/> class, ensuring its functionality
/// for application bootstrapping with ReactiveUI.
/// </summary>
[TestFixture]
public class AppBootstrapperTests
{
    /// <summary>
    /// Validates that AppBootstrapper can be instantiated without errors.
    /// </summary>
    [Test]
    public void Constructor_CreatesInstance_Successfully()
    {
        AppBootstrapper? bootstrapper = null;

        Assert.DoesNotThrow(() => bootstrapper = new AppBootstrapper());
        Assert.That(bootstrapper, Is.Not.Null);
    }

    /// <summary>
    /// Validates that AppBootstrapper implements IScreen interface.
    /// </summary>
    [Test]
    public void AppBootstrapper_ImplementsIScreen()
    {
        var bootstrapper = new AppBootstrapper();
        Assert.That(bootstrapper, Is.InstanceOf<IScreen>());
    }

    /// <summary>
    /// Validates that Router property is initialized and not null.
    /// </summary>
    [Test]
    public void Router_IsInitialized_NotNull()
    {
        var bootstrapper = new AppBootstrapper();
        Assert.That(bootstrapper.Router, Is.Not.Null);
        Assert.That(bootstrapper.Router, Is.InstanceOf<RoutingState>());
    }

    /// <summary>
    /// Validates that AppBootstrapper inherits from ReactiveObject.
    /// </summary>
    [Test]
    public void AppBootstrapper_InheritsFromReactiveObject()
    {
        var bootstrapper = new AppBootstrapper();
        Assert.That(bootstrapper, Is.InstanceOf<ReactiveObject>());
    }

    /// <summary>
    /// Validates that multiple instances can be created.
    /// </summary>
    [Test]
    public void Constructor_AllowsMultipleInstances()
    {
        var bootstrapper1 = new AppBootstrapper();
        var bootstrapper2 = new AppBootstrapper();

        using (Assert.EnterMultipleScope())
        {
            Assert.That(bootstrapper1, Is.Not.Null);
            Assert.That(bootstrapper2, Is.Not.Null);
            Assert.That(bootstrapper1, Is.Not.SameAs(bootstrapper2));
            Assert.That(bootstrapper1.Router, Is.Not.SameAs(bootstrapper2.Router));
        }
    }
}

// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ReactiveUI;
using ReactiveUI.Uno;

namespace ReactiveUI.Uno.Tests;

/// <summary>
/// RegistrationsTests.
/// </summary>
[TestFixture]
public class RegistrationsTests
{
    /// <summary>
    /// Resets the schedulers.
    /// </summary>
    [SetUp]
    public void ResetSchedulers()
    {
        // Reset to defaults before each test to avoid cross-test pollution.
        RxApp.MainThreadScheduler = null!;
        RxApp.TaskpoolScheduler = null!;
    }

    /// <summary>
    /// Registers the registers expected services and configures schedulers.
    /// </summary>
    [Test]
    public void Register_RegistersExpectedServices_And_ConfiguresSchedulers()
    {
        var registered = new List<Type>();

        void Register(Func<object> factory, Type serviceType)
        {
            // Don't invoke the factory here to avoid loading UI types in headless tests.
            registered.Add(serviceType);
        }

        var sut = new Registrations();
        sut.Invoking(x => x.Register(Register)).Should().NotThrow();

        // Verify the set of required service interfaces are registered
        registered.Should().Contain(typeof(IPlatformOperations));
        registered.Should().Contain(typeof(IActivationForViewFetcher));
        registered.Should().Contain(typeof(ICreatesObservableForProperty));
        registered.Should().Contain(typeof(IPropertyBindingHook));
        registered.Should().Contain(typeof(ISuspensionDriver));

        // The implementation registers 16 binding converters + 2 hooks/services + 3 core services = 21 total
        // (String/byte(short/int/long/float/double/decimal) + nullable versions + BooleanToVisibility)
        registered.Count.Should().Be(21);

        // Verify schedulers are set to safe defaults for headless environment
        RxApp.TaskpoolScheduler.Should().BeSameAs(System.Reactive.Concurrency.TaskPoolScheduler.Default);
        RxApp.MainThreadScheduler.Should().BeSameAs(System.Reactive.Concurrency.CurrentThreadScheduler.Instance);
    }

    /// <summary>
    /// Registers the throws on null register function.
    /// </summary>
    [Test]
    public void Register_Throws_On_Null_RegisterFunction()
    {
        var sut = new Registrations();
        Action act = () => sut.Register(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}

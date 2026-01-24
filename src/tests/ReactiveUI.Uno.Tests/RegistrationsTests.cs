// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NSubstitute;
using ReactiveUI.Uno;
using Splat;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests;

/// <summary>
/// Contains tests for the <see cref="ReactiveUI.Uno.Registrations"/> class, specifically validating
/// its functionality for registering services.
/// </summary>
public class RegistrationsTests
{
    /// <summary>
    /// Validates that Register throws ArgumentNullException when registrar is null.
    /// </summary>
    [Test]
    public async Task Register_ThrowsArgumentNullException_WhenRegistrarIsNull()
    {
        Registrations sut = new();
        await Assert.That(() => sut.Register(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Validates that Registrations implements IWantsToRegisterStuff.
    /// </summary>
    [Test]
    public async Task Registrations_ImplementsIWantsToRegisterStuff()
    {
        Registrations sut = new();
        await Assert.That(sut).IsAssignableTo<IWantsToRegisterStuff>();
    }

    /// <summary>
    /// Validates that Registrations can be instantiated.
    /// </summary>
    [Test]
    public async Task Registrations_CanBeInstantiated()
    {
        Registrations sut = new();
        await Assert.That(sut).IsNotNull();
    }

    /// <summary>
    /// Validates that Register calls registrar methods.
    /// </summary>
    [Test]
    public async Task Register_CallsRegistrarMethods()
    {
        var registrar = Substitute.For<IRegistrar>();
        Registrations sut = new();

        sut.Register(registrar);

        // Verify that RegisterConstant was called multiple times
        registrar.Received().RegisterConstant(Arg.Any<Func<IPlatformOperations>>());
        registrar.Received().RegisterConstant(Arg.Any<Func<IActivationForViewFetcher>>());
        registrar.Received().RegisterConstant(Arg.Any<Func<ICreatesObservableForProperty>>());
        registrar.Received().RegisterConstant(Arg.Any<Func<IPropertyBindingHook>>());
        registrar.Received().RegisterConstant(Arg.Any<Func<ISuspensionDriver>>());
        await Task.CompletedTask;
    }

    /// <summary>
    /// Validates that Register can be called multiple times without error.
    /// </summary>
    [Test]
    public async Task Register_CanBeCalledMultipleTimes()
    {
        var registrar = Substitute.For<IRegistrar>();
        Registrations sut = new();

        sut.Register(registrar);
        sut.Register(registrar);

        await Assert.That(() => sut.Register(registrar)).ThrowsNothing();
    }
}

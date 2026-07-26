// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NSubstitute;
#if REACTIVE_SHIM
using UnoRegistrations = ReactiveUI.Uno.Reactive.Registrations;
#else
using UnoRegistrations = ReactiveUI.Uno.Registrations;
#endif

namespace ReactiveUI.Uno.Tests;

/// <summary>Tests Uno service registrations.</summary>
public class RegistrationsTests
{
    /// <summary>The expected number of binding converter registrations.</summary>
    private const int ExpectedBindingTypeConverterRegistrationCount = 16;

    /// <summary>Validates that Register throws ArgumentNullException when registrar is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Register_ThrowsArgumentNullException_WhenRegistrarIsNull()
    {
        UnoRegistrations sut = new();
        await Assert.That(() => sut.Register(null!)).Throws<ArgumentNullException>();
    }

    /// <summary>Validates that Registrations implements IWantsToRegisterStuff.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Registrations_ImplementsIWantsToRegisterStuff()
    {
        UnoRegistrations sut = new();
        await Assert.That(sut).IsAssignableTo<IWantsToRegisterStuff>();
    }

    /// <summary>Validates that Registrations can be instantiated.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Registrations_CanBeInstantiated()
    {
        UnoRegistrations sut = new();
        await Assert.That(sut).IsNotNull();
    }

    /// <summary>Validates that Register calls registrar methods.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Register_CallsRegistrarMethods()
    {
        var registrar = Substitute.For<IRegistrar>();
        UnoRegistrations sut = new();

        sut.Register(registrar);

        // Verify that RegisterConstant was called multiple times
        registrar.Received().RegisterConstant(Arg.Any<Func<IPlatformOperations>>());
        registrar.Received().RegisterConstant(Arg.Any<Func<IActivationForViewFetcher>>());
        registrar.Received().RegisterConstant(Arg.Any<Func<ICreatesObservableForProperty>>());
        registrar.Received().RegisterConstant(Arg.Any<Func<IPropertyBindingHook>>());
        registrar.Received().RegisterConstant(Arg.Any<Func<ISuspensionDriver>>());
        await Task.CompletedTask;
    }

    /// <summary>Validates that Register can be called multiple times without error.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Register_CanBeCalledMultipleTimes()
    {
        var registrar = Substitute.For<IRegistrar>();
        UnoRegistrations sut = new();

        sut.Register(registrar);
        sut.Register(registrar);

        await Assert.That(() => sut.Register(registrar)).ThrowsNothing();
    }

    /// <summary>Validates that registration delegates can be invoked to create service instances.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Register_Delegates_CreateExpectedServiceInstances()
    {
        var registrar = Substitute.For<IRegistrar>();
        UnoRegistrations sut = new();

        registrar
            .When(x => x.RegisterConstant(Arg.Any<Func<IPlatformOperations>>()))
            .Do(x => _ = x.Arg<Func<IPlatformOperations>>()!());
        registrar
            .When(x => x.RegisterConstant(Arg.Any<Func<IActivationForViewFetcher>>()))
            .Do(x => _ = x.Arg<Func<IActivationForViewFetcher>>()!());
        registrar
            .When(x => x.RegisterConstant(Arg.Any<Func<ICreatesObservableForProperty>>()))
            .Do(x => _ = x.Arg<Func<ICreatesObservableForProperty>>()!());
        registrar
            .When(x => x.RegisterConstant(Arg.Any<Func<IPropertyBindingHook>>()))
            .Do(x => _ = x.Arg<Func<IPropertyBindingHook>>()!());
        registrar
            .When(x => x.RegisterConstant(Arg.Any<Func<ISuspensionDriver>>()))
            .Do(x => _ = x.Arg<Func<ISuspensionDriver>>()!());
        registrar
            .When(x => x.RegisterConstant(Arg.Any<Func<IBindingTypeConverter>>()))
            .Do(x => _ = x.Arg<Func<IBindingTypeConverter>>()!());

        sut.Register(registrar);

        registrar.Received(1).RegisterConstant(Arg.Any<Func<IPlatformOperations>>());
        registrar.Received(1).RegisterConstant(Arg.Any<Func<IActivationForViewFetcher>>());
        registrar.Received(1).RegisterConstant(Arg.Any<Func<ICreatesObservableForProperty>>());
        registrar.Received(1).RegisterConstant(Arg.Any<Func<IPropertyBindingHook>>());
        registrar.Received(1).RegisterConstant(Arg.Any<Func<ISuspensionDriver>>());
        registrar.Received(ExpectedBindingTypeConverterRegistrationCount)
            .RegisterConstant(Arg.Any<Func<IBindingTypeConverter>>());
        await Assert.That(RxSchedulers.SuppressViewCommandBindingMessage).IsTrue();
    }
}

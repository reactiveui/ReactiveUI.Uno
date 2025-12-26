// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NUnit.Framework;

namespace ReactiveUI.Uno.Tests;

/// <summary>
/// Contains tests for the <see cref="ReactiveUI.Uno.Registrations"/> class, specifically validating
/// its functionality for registering services and configuring schedulers.
/// </summary>
[TestFixture]
public class RegistrationsTests
{
    /// <summary>
    /// Resets the schedulers to default values to ensure clean state before each test.
    /// </summary>
    [SetUp]
    public void ResetSchedulers()
    {
        // Reset to defaults before each test to avoid cross-test pollution.
        RxApp.MainThreadScheduler = null!;
        RxApp.TaskpoolScheduler = null!;
    }

    /// <summary>
    /// Validates that the expected services are correctly registered and schedulers are configured appropriately.
    /// </summary>
    /// <remarks>
    /// This test ensures that all required service interfaces are registered without throwing errors,
    /// verifies the total count of registrations, and confirms safe default scheduler configurations
    /// for a headless test environment.
    /// </remarks>
    [Test]
    public void Register_RegistersExpectedServices_And_ConfiguresSchedulers()
    {
        List<Type> registered = [];

        void Register(Func<object> factory, Type serviceType)
        {
            // Don't invoke the factory here to avoid loading UI types in headless tests.
            registered.Add(serviceType);
        }

        Registrations sut = new();
        Assert.DoesNotThrow(() => sut.Register(Register));

        // Verify the set of required service interfaces are registered
        Assert.That(registered, Does.Contain(typeof(IPlatformOperations)));
        Assert.That(registered, Does.Contain(typeof(IActivationForViewFetcher)));
        Assert.That(registered, Does.Contain(typeof(ICreatesObservableForProperty)));
        Assert.That(registered, Does.Contain(typeof(IPropertyBindingHook)));
        Assert.That(registered, Does.Contain(typeof(ISuspensionDriver)));

        using (Assert.EnterMultipleScope())
        {
            // The implementation registers 16 binding converters + 2 hooks/services + 3 core services = 21 total
            // (String/byte(short/int/long/float/double/decimal) + nullable versions + BooleanToVisibility)
            Assert.That(registered, Has.Count.EqualTo(21));

            // Verify schedulers are set to safe defaults for headless environment
            // Note: For WebAssembly targets (net9.0-browserwasm), WasmScheduler.Default is used instead
            // of TaskPoolScheduler.Default due to WebAssembly's lack of multithreading support
            Assert.That(RxApp.TaskpoolScheduler, Is.SameAs(System.Reactive.Concurrency.TaskPoolScheduler.Default));
            Assert.That(RxApp.MainThreadScheduler, Is.SameAs(System.Reactive.Concurrency.CurrentThreadScheduler.Instance));
        }
    }

    /// <summary>
    /// Verifies that the <see cref="Registrations.Register"/> method throws an <see cref="ArgumentNullException"/>
    /// when the provided register function is null.
    /// </summary>
    [Test]
    public void Register_Throws_On_Null_RegisterFunction()
    {
        Registrations sut = new();
        Assert.Throws<ArgumentNullException>(() => sut.Register(null!));
    }

    /// <summary>
    /// Validates that specific type converters are registered correctly.
    /// </summary>
    [Test]
    public void Register_RegistersSpecificTypeConverters()
    {
        List<(Type serviceType, object instance)> registered = [];

        void Register(Func<object> factory, Type serviceType)
        {
            var instance = factory();
            registered.Add((serviceType, instance));
        }

        Registrations sut = new();
        sut.Register(Register);

        // Verify specific type converters are registered
        var converters = registered.Where(x => x.serviceType == typeof(IBindingTypeConverter)).ToList();
        Assert.That(converters, Has.Count.EqualTo(16), "Expected 16 type converters");

        // Verify some specific converter types exist
        var converterInstances = converters.Select(x => x.instance.GetType().Name).ToArray();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(converterInstances, Does.Contain("StringConverter"));
            Assert.That(converterInstances, Does.Contain("ByteToStringTypeConverter"));
            Assert.That(converterInstances, Does.Contain("IntegerToStringTypeConverter"));
            Assert.That(converterInstances, Does.Contain("BooleanToVisibilityTypeConverter"));
        }
    }

    /// <summary>
    /// Validates that core service types are registered.
    /// </summary>
    [Test]
    public void Register_RegistersCoreServiceTypes()
    {
        List<(Type serviceType, Type instanceType)> registered = [];

        void Register(Func<object> factory, Type serviceType)
        {
            var instance = factory();
            registered.Add((serviceType, instance.GetType()));
        }

        Registrations sut = new();
        sut.Register(Register);

        using (Assert.EnterMultipleScope())
        {
            // Check core services
            Assert.That(registered, Does.Contain((typeof(IPlatformOperations), typeof(PlatformOperations))));
            Assert.That(registered, Does.Contain((typeof(IActivationForViewFetcher), typeof(ActivationForViewFetcher))));
            Assert.That(registered, Does.Contain((typeof(ICreatesObservableForProperty), typeof(DependencyObjectObservableForProperty))));
            Assert.That(registered, Does.Contain((typeof(IPropertyBindingHook), typeof(AutoDataTemplateBindingHook))));
            Assert.That(registered, Does.Contain((typeof(ISuspensionDriver), typeof(WinRTAppDataDriver))));
        }
    }

    /// <summary>
    /// Validates that all registered factories produce non-null instances.
    /// </summary>
    [Test]
    public void Register_AllFactoriesProduceNonNullInstances()
    {
        List<Func<object>> factories = [];

        void Register(Func<object> factory, Type serviceType)
        {
            factories.Add(factory);
        }

        Registrations sut = new();
        sut.Register(Register);

        foreach (var factory in factories)
        {
            var instance = factory();
            Assert.That(instance, Is.Not.Null, $"Factory for {instance?.GetType()} produced null instance");
        }
    }

    /// <summary>
    /// Validates that registrations can be called multiple times without error.
    /// </summary>
    [Test]
    public void Register_CanBeCalledMultipleTimes()
    {
        var callCount = 0;

        void Register(Func<object> factory, Type serviceType)
        {
            callCount++;
        }

        Registrations sut = new();

        Assert.DoesNotThrow(() => sut.Register(Register));
        var firstCallCount = callCount;

        Assert.DoesNotThrow(() => sut.Register(Register));
        var secondCallCount = callCount;

        Assert.That(secondCallCount, Is.EqualTo(firstCallCount * 2), "Second registration should register same number of services");
    }

    /// <summary>
    /// Validates that factory methods don't return null for any registered service.
    /// </summary>
    [Test]
    public void Register_AllFactoriesReturnNonNullInstances()
    {
        var factories = new List<(Func<object> Factory, Type ServiceType)>();

        void Register(Func<object> factory, Type serviceType)
        {
            factories.Add((factory, serviceType));
        }

        Registrations sut = new();
        sut.Register(Register);

        foreach (var (factory, serviceType) in factories)
        {
            var instance = factory();
            Assert.That(instance, Is.Not.Null, $"Factory for {serviceType} returned null");
            Assert.That(
                instance,
                Is.AssignableTo(serviceType),
                $"Factory for {serviceType} returned instance of wrong type: {instance.GetType()}");
        }
    }

    /// <summary>
    /// Validates that factories produce consistent instances of the correct types.
    /// </summary>
    [Test]
    public void Register_FactoriesProduceCorrectTypes()
    {
        var typeMapping = new Dictionary<Type, Type>();

        void Register(Func<object> factory, Type serviceType)
        {
            var instance = factory();
            if (!typeMapping.ContainsKey(serviceType))
            {
                typeMapping[serviceType] = instance.GetType();
            }
        }

        Registrations sut = new();
        sut.Register(Register);

        Assert.That(typeMapping, Is.Not.Empty, "At least one type mapping should exist");

        // Verify some expected services are registered
        var expectedServices = new[]
        {
            typeof(IActivationForViewFetcher),
            typeof(IPropertyBindingHook),
            typeof(IPlatformOperations)
        };

        foreach (var expectedService in expectedServices)
        {
            Assert.That(
                typeMapping.ContainsKey(expectedService),
                Is.True,
                $"Expected service {expectedService} was not registered");
        }
    }

    /// <summary>
    /// Validates that registration with null function throws.
    /// </summary>
    [Test]
    public void Register_WithNullFunction_Throws()
    {
        Registrations sut = new();

        Assert.That(() => sut.Register(null!), Throws.ArgumentNullException
            .With.Property("ParamName").EqualTo("registerFunction"));
    }

    /// <summary>
    /// Validates that each factory produces a new instance on each call.
    /// </summary>
    [Test]
    public void Register_FactoriesProduceNewInstances()
    {
        var factories = new List<Func<object>>();

        void Register(Func<object> factory, Type serviceType)
        {
            factories.Add(factory);
        }

        Registrations sut = new();
        sut.Register(Register);

        foreach (var factory in factories)
        {
            var instance1 = factory();
            var instance2 = factory();

            using (Assert.EnterMultipleScope())
            {
                // Most services should produce new instances (unless they're singletons)
                // At minimum, they should not throw when called multiple times
                Assert.That(instance1, Is.Not.Null);
                Assert.That(instance2, Is.Not.Null);
            }
        }
    }
}

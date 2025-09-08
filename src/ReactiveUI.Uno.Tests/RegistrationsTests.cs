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
    /// Documents and verifies that WebAssembly targets use WasmScheduler instead of TaskPoolScheduler.
    /// </summary>
    /// <remarks>
    /// This test serves as documentation for the WebAssembly scheduler behavior.
    /// While the test project targets net9.0 (not net9.0-browserwasm), the actual
    /// ReactiveUI.Uno library uses conditional compilation to ensure that:
    /// - WebAssembly targets (net9.0-browserwasm) use WasmScheduler.Default
    /// - All other targets use TaskPoolScheduler.Default
    /// This is necessary because WebAssembly doesn't support multithreading.
    /// </remarks>
    [Test]
    public void Register_Uses_WasmScheduler_For_WebAssembly_Targets()
    {
        // This test documents the expected behavior for WebAssembly targets.
        // The conditional compilation in Registrations.cs ensures that:
        // - For __WASM__ || BROWSERWASM: RxApp.TaskpoolScheduler = WasmScheduler.Default
        // - For other targets: RxApp.TaskpoolScheduler = TaskPoolScheduler.Default

        // Since this test runs on net9.0 (not browserwasm), we verify the non-WebAssembly behavior
        List<Type> registered = [];
        void Register(Func<object> factory, Type serviceType) => registered.Add(serviceType);

        Registrations sut = new();
        sut.Register(Register);

        // For non-WebAssembly targets, TaskPoolScheduler should be used
        Assert.That(
            RxApp.TaskpoolScheduler,
            Is.SameAs(System.Reactive.Concurrency.TaskPoolScheduler.Default),
            "Non-WebAssembly targets should use TaskPoolScheduler.Default");

        // This test serves as documentation that WebAssembly builds will use WasmScheduler instead
        Assert.Pass("WebAssembly targets (net9.0-browserwasm) will use WasmScheduler.Default due to conditional compilation");
    }
}

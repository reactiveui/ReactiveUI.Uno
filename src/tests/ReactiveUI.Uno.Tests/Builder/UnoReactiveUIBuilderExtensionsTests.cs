// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using NSubstitute;
using ReactiveUI.Builder;
using Splat;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Builder;

/// <summary>
/// Contains tests for the <see cref="UnoReactiveUIBuilderExtensions"/> class, ensuring its functionality
/// for builder pattern extensions.
/// </summary>
public class UnoReactiveUIBuilderExtensionsTests
{
    /// <summary>
    /// Validates that WithUno throws ArgumentNullException when builder is null.
    /// </summary>
    [Test]
    public async Task WithUno_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithUno(null!, new())).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("builder");
    }

    /// <summary>
    /// Validates that WithUnoScheduler throws ArgumentNullException when builder is null.
    /// </summary>
    [Test]
    public async Task WithUnoScheduler_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithUnoScheduler(null!)).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("builder");
    }

    /// <summary>
    /// Validates that WithDefaultIScreen throws ArgumentNullException when builder is null.
    /// </summary>
    [Test]
    public async Task WithDefaultIScreen_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithDefaultIScreen(null!)).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("builder");
    }

    /// <summary>
    /// Validates that WithUnoScheduler returns the builder for chaining.
    /// Note: This test may fail in headless environments due to scheduler initialization.
    /// </summary>
    [Test]
    public async Task WithUnoScheduler_ReturnsBuilder_ForChaining()
    {
        try
        {
            var builder = Substitute.For<IReactiveUIBuilder>();
            builder.WithMainThreadScheduler(Arg.Any<IScheduler>()).Returns(builder);

            var result = builder.WithUnoScheduler();

            await Assert.That(result).IsSameReferenceAs(builder);
        }
        catch (TypeInitializationException)
        {
            Skip.Test("UI context not available for scheduler initialization in headless environment.");
        }
    }

    /// <summary>
    /// Validates that WithUnoScheduler calls WithMainThreadScheduler on the builder.
    /// Note: This test may fail in headless environments due to scheduler initialization.
    /// </summary>
    [Test]
    public async Task WithUnoScheduler_CallsWithMainThreadScheduler()
    {
        try
        {
            var builder = Substitute.For<IReactiveUIBuilder>();
            builder.WithMainThreadScheduler(Arg.Any<IScheduler>()).Returns(builder);

            builder.WithUnoScheduler();

            builder.Received(1).WithMainThreadScheduler(Arg.Any<IScheduler>());
            await Task.CompletedTask;
        }
        catch (TypeInitializationException)
        {
            Skip.Test("UI context not available for scheduler initialization in headless environment.");
        }
    }

    /// <summary>
    /// Validates that WithDefaultIScreen returns the builder for chaining.
    /// </summary>
    [Test]
    public async Task WithDefaultIScreen_ReturnsBuilder_ForChaining()
    {
        var builder = Substitute.For<IReactiveUIBuilder>();
        builder.WithRegistration(Arg.Any<Action<IMutableDependencyResolver>>()).Returns(builder);

        var result = builder.WithDefaultIScreen();

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>
    /// Validates that WithDefaultIScreen calls WithRegistration.
    /// </summary>
    [Test]
    public async Task WithDefaultIScreen_CallsWithRegistration()
    {
        var builder = Substitute.For<IReactiveUIBuilder>();
        builder.WithRegistration(Arg.Any<Action<IMutableDependencyResolver>>()).Returns(builder);

        builder.WithDefaultIScreen();

        builder.Received(1).WithRegistration(Arg.Any<Action<IMutableDependencyResolver>>());
        await Task.CompletedTask;
    }

    /// <summary>
    /// Validates that BuildApp returns the instance when successful.
    /// </summary>
    [Test]
    public async Task BuildApp_ReturnsInstance_WhenSuccessful()
    {
        var builder = Substitute.For<IReactiveUIBuilder>();
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);
        builder.Build().Returns(instance);

        var result = builder.BuildApp();

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Current).IsNotNull();
    }

    /// <summary>
    /// Validates that BuildApp validates instance is not null.
    /// </summary>
    [Test]
    public async Task BuildApp_ValidatesInstanceNotNull()
    {
        var builder = Substitute.For<IReactiveUIBuilder>();
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);
        builder.Build().Returns(instance);

        var result = builder.BuildApp();

        await Assert.That(result).IsNotNull();
    }

    /// <summary>
    /// Validates that the UnoMainThreadScheduler property exists.
    /// Note: Accessing the property may fail in headless environment due to type initialization.
    /// </summary>
    [Test]
    public async Task UnoMainThreadScheduler_PropertyExists()
    {
        var type = typeof(UnoReactiveUIBuilderExtensions);
#if WINDOWS
        var property = type.GetProperty("UnoWinUIMainThreadScheduler");
#else
        var property = type.GetProperty("UnoMainThreadScheduler");
#endif
        await Assert.That(property).IsNotNull();
        await Assert.That(typeof(IScheduler).IsAssignableFrom(property!.PropertyType)).IsTrue();
    }

    /// <summary>
    /// Validates that extension methods are defined in the correct namespace.
    /// </summary>
    [Test]
    public async Task ExtensionMethods_AreInCorrectNamespace()
    {
        var type = typeof(UnoReactiveUIBuilderExtensions);

        await Assert.That(type.Namespace).IsEqualTo("ReactiveUI.Builder");
    }

    /// <summary>
    /// Validates that UnoReactiveUIBuilderExtensions is a static class.
    /// </summary>
    [Test]
    public async Task UnoReactiveUIBuilderExtensions_IsStaticClass()
    {
        var type = typeof(UnoReactiveUIBuilderExtensions);

        await Assert.That(type.IsAbstract).IsTrue();
        await Assert.That(type.IsSealed).IsTrue();
    }

    /// <summary>
    /// Validates WithDefaultIScreen has RequiresUnreferencedCode attribute.
    /// </summary>
    [Test]
    public async Task WithDefaultIScreen_HasRequiresUnreferencedCodeAttribute()
    {
        var method = typeof(UnoReactiveUIBuilderExtensions).GetMethod(nameof(UnoReactiveUIBuilderExtensions.WithDefaultIScreen));
        var attribute = method?.GetCustomAttributes(typeof(System.Diagnostics.CodeAnalysis.RequiresUnreferencedCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>
    /// Validates that WithUnoScheduler method exists.
    /// </summary>
    [Test]
    public async Task WithUnoScheduler_MethodExists()
    {
        var method = typeof(UnoReactiveUIBuilderExtensions).GetMethod(nameof(UnoReactiveUIBuilderExtensions.WithUnoScheduler));

        await Assert.That(method).IsNotNull();
    }

    /// <summary>
    /// Validates that WithUno method exists.
    /// </summary>
    [Test]
    public async Task WithUno_MethodExists()
    {
        var method = typeof(UnoReactiveUIBuilderExtensions).GetMethod(nameof(UnoReactiveUIBuilderExtensions.WithUno));

        await Assert.That(method).IsNotNull();
    }

    /// <summary>
    /// Validates that WithDefaultIScreen method exists.
    /// </summary>
    [Test]
    public async Task WithDefaultIScreen_MethodExists()
    {
        var method = typeof(UnoReactiveUIBuilderExtensions).GetMethod(nameof(UnoReactiveUIBuilderExtensions.WithDefaultIScreen));

        await Assert.That(method).IsNotNull();
    }
}

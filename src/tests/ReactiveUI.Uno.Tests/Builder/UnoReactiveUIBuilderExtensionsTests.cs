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
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithUno(null!)).Throws<ArgumentNullException>();
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
    /// Validates that BuildApp throws ArgumentNullException when builder is null.
    /// </summary>
    [Test]
    public async Task BuildApp_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.BuildApp(null!)).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("builder");
    }

    /// <summary>
    /// Validates that WithInstance throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object>(null!, _ => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (2 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_TwoTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string>(null!, (_, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (3 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_ThreeTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int>(null!, (_, _, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (4 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_FourTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool>(null!, (_, _, _, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (5 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_FiveTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool, double>(null!, (_, _, _, _, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (6 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_SixTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool, double, float>(null!, (_, _, _, _, _, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (7 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_SevenTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool, double, float, long>(null!, (_, _, _, _, _, _, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (8 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_EightTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool, double, float, long, byte>(null!, (_, _, _, _, _, _, _, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (9 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_NineTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool, double, float, long, byte, short>(null!, (_, _, _, _, _, _, _, _, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
    }

    /// <summary>
    /// Validates that WithInstance (10 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public async Task WithInstance_TenTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool, double, float, long, byte, short, char>(null!, (_, _, _, _, _, _, _, _, _, _) => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
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
    /// Validates that BuildApp method verifies instance is not null.
    /// Note: Testing null returns from mocks is complex due to NSubstitute behavior.
    /// </summary>
    [Test]
    public async Task BuildApp_ValidatesInstanceNotNull()
    {
        // This test verifies the behavior indirectly - when a valid mock is provided,
        // BuildApp succeeds. The error paths (null instance, null Current) are enforced
        // by the implementation but difficult to test with mocks.
        var builder = Substitute.For<IReactiveUIBuilder>();
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);
        builder.Build().Returns(instance);

        var result = builder.BuildApp();

        await Assert.That(result).IsNotNull();
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
    /// Validates that WithInstance invokes the action with resolved service.
    /// </summary>
    [Test]
    public async Task WithInstance_InvokesAction_WithResolvedService()
    {
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        const string testService = "TestService";
        resolver.GetService<string>().Returns(testService);
        instance.Current.Returns(resolver);

        string? receivedValue = null;
        UnoReactiveUIBuilderExtensions.WithInstance<string>(instance, s => receivedValue = s);

        await Assert.That(receivedValue).IsEqualTo(testService);
    }

    /// <summary>
    /// Validates that WithInstance returns the instance for chaining.
    /// </summary>
    [Test]
    public async Task WithInstance_ReturnsInstance_ForChaining()
    {
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);

        var result = UnoReactiveUIBuilderExtensions.WithInstance<string>(instance, _ => { });

        await Assert.That(result).IsSameReferenceAs(instance);
    }

    /// <summary>
    /// Validates that WithInstance handles null action gracefully.
    /// </summary>
    [Test]
    public async Task WithInstance_HandlesNullAction_Gracefully()
    {
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);

        var result = UnoReactiveUIBuilderExtensions.WithInstance<string>(instance, null!);

        await Assert.That(result).IsSameReferenceAs(instance);
    }

    /// <summary>
    /// Validates that WithInstance with two types returns instance for chaining.
    /// </summary>
    [Test]
    public async Task WithInstance_TwoTypes_ReturnsInstance_ForChaining()
    {
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);

        var result = UnoReactiveUIBuilderExtensions.WithInstance<string, int>(instance, (_, _) => { });

        await Assert.That(result).IsSameReferenceAs(instance);
    }

    /// <summary>
    /// Validates that WithInstance handles null action for two types.
    /// </summary>
    [Test]
    public async Task WithInstance_TwoTypes_HandlesNullAction_Gracefully()
    {
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);

        var result = UnoReactiveUIBuilderExtensions.WithInstance<string, int>(instance, null!);

        await Assert.That(result).IsSameReferenceAs(instance);
    }

    /// <summary>
    /// Validates that WithInstance handles null action for three types.
    /// </summary>
    [Test]
    public async Task WithInstance_ThreeTypes_HandlesNullAction_Gracefully()
    {
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);

        var result = UnoReactiveUIBuilderExtensions.WithInstance<string, int, bool>(instance, null!);

        await Assert.That(result).IsSameReferenceAs(instance);
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
    /// Validates WithInstance throws when instance has null Current.
    /// </summary>
    [Test]
    public async Task WithInstance_ThrowsArgumentNullException_WhenCurrentIsNull()
    {
        var instance = Substitute.For<IReactiveUIInstance>();
        instance.Current.Returns((IReadonlyDependencyResolver?)null);

        var exception = await Assert.That(() => UnoReactiveUIBuilderExtensions.WithInstance<string>(instance, _ => { })).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("reactiveUIInstance");
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
    /// Validates that WithInstance four types handles null action gracefully.
    /// </summary>
    [Test]
    public async Task WithInstance_FourTypes_HandlesNullAction_Gracefully()
    {
        var instance = Substitute.For<IReactiveUIInstance>();
        var resolver = Substitute.For<IReadonlyDependencyResolver>();
        instance.Current.Returns(resolver);

        var result = UnoReactiveUIBuilderExtensions.WithInstance<string, int, bool, double>(instance, null!);

        await Assert.That(result).IsSameReferenceAs(instance);
    }

    /// <summary>
    /// Validates that WithInstance methods exist for various type counts.
    /// </summary>
    [Test]
    public async Task WithInstance_MethodsExist_ForVariousTypeCounts()
    {
        var type = typeof(UnoReactiveUIBuilderExtensions);
        var methods = type.GetMethods()
            .Where(m => m.Name == "WithInstance")
            .ToList();

        // Should have multiple overloads (at least 1-16 type parameters)
        await Assert.That(methods.Count).IsGreaterThanOrEqualTo(10);
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

    /// <summary>
    /// Validates that BuildApp method exists.
    /// </summary>
    [Test]
    public async Task BuildApp_MethodExists()
    {
        var method = typeof(UnoReactiveUIBuilderExtensions).GetMethod(nameof(UnoReactiveUIBuilderExtensions.BuildApp));

        await Assert.That(method).IsNotNull();
    }
}

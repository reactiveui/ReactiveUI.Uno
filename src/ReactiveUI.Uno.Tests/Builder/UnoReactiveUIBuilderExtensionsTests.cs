// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NUnit.Framework;
using ReactiveUI.Builder;
using Splat;

namespace ReactiveUI.Uno.Tests.Builder;

/// <summary>
/// Contains tests for the <see cref="UnoReactiveUIBuilderExtensions"/> class, ensuring its functionality
/// for builder pattern extensions.
/// </summary>
[TestFixture]
public class UnoReactiveUIBuilderExtensionsTests
{
    /// <summary>
    /// Validates that BuildApp throws ArgumentNullException when builder is null.
    /// </summary>
    [Test]
    public void BuildApp_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.That(
            () => UnoReactiveUIBuilderExtensions.BuildApp(null!),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("builder"));
    }

    /// <summary>
    /// Validates that WithDefaultIScreen throws ArgumentNullException when builder is null.
    /// </summary>
    [Test]
    public void WithDefaultIScreen_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.That(
            () => UnoReactiveUIBuilderExtensions.WithDefaultIScreen(null!),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("builder"));
    }

    /// <summary>
    /// Validates that WithInstance throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public void WithInstance_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        Assert.That(
            () => UnoReactiveUIBuilderExtensions.WithInstance<object>(null!, _ => { }),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("reactiveUIInstance"));
    }

    /// <summary>
    /// Validates that WithInstance (2 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public void WithInstance_TwoTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        Assert.That(
            () => UnoReactiveUIBuilderExtensions.WithInstance<object, string>(null!, (_, _) => { }),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("reactiveUIInstance"));
    }

    /// <summary>
    /// Validates that WithInstance (3 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public void WithInstance_ThreeTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        Assert.That(
            () => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int>(null!, (_, _, _) => { }),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("reactiveUIInstance"));
    }

    /// <summary>
    /// Validates that WithInstance (4 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public void WithInstance_FourTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        Assert.That(
            () => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool>(null!, (_, _, _, _) => { }),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("reactiveUIInstance"));
    }

    /// <summary>
    /// Validates that WithInstance (5 types) throws ArgumentNullException when reactiveUIInstance is null.
    /// </summary>
    [Test]
    public void WithInstance_FiveTypes_ThrowsArgumentNullException_WhenReactiveUIInstanceIsNull()
    {
        Assert.That(
            () => UnoReactiveUIBuilderExtensions.WithInstance<object, string, int, bool, double>(null!, (_, _, _, _, _) => { }),
            Throws.ArgumentNullException.With.Property("ParamName").EqualTo("reactiveUIInstance"));
    }
}

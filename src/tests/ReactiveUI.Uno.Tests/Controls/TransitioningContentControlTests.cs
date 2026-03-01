// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using Microsoft.UI.Xaml.Controls;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Controls;

/// <summary>
/// Tests for TransitioningContentControl functionality.
/// </summary>
public class TransitioningContentControlTests
{
    /// <summary>
    /// Setup for each test.
    /// </summary>
    [Before(Test)]
    public void SetUp()
    {
        // Skip tests if no UI context is available (headless environment)
        try
        {
            var window = Microsoft.UI.Xaml.Window.Current;
            if (window is null)
            {
                Skip.Test("Skipping test because no UI context is available (headless environment)");
            }
        }
        catch (TypeInitializationException)
        {
            Skip.Test("Skipping test because no UI context is available (headless environment)");
        }
        catch (NotSupportedException)
        {
            Skip.Test("Skipping test because no UI context is available (headless environment)");
        }
    }

    /// <summary>
    /// Test constructor creates instance successfully.
    /// </summary>
    [Test]
    public async Task Constructor_CreatesInstanceSuccessfully()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        await Assert.That(control).IsNotNull();
        await Assert.That(control).IsAssignableTo<ContentControl>();
    }

    /// <summary>
    /// Test control inherits from ContentControl.
    /// </summary>
    [Test]
    public async Task Control_InheritsFromContentControl()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        await Assert.That(control).IsAssignableTo<ContentControl>();
    }

    /// <summary>
    /// Test control is partial class (compile-time verification).
    /// </summary>
    [Test]
    public async Task Control_IsPartialClass()
    {
        // This test ensures the class compiles correctly as a partial class
        var control = new TransitioningContentControl();
        await Assert.That(control).IsNotNull();
    }

    /// <summary>
    /// Test multiple instantiation creates separate objects.
    /// </summary>
    [Test]
    public async Task MultipleInstantiation_CreatesSeparateObjects()
    {
        // Act
        var control1 = new TransitioningContentControl();
        var control2 = new TransitioningContentControl();

        // Assert
        await Assert.That(control1).IsNotSameReferenceAs(control2);
    }

    /// <summary>
    /// Test control can be used as ContentControl.
    /// </summary>
    [Test]
    public async Task Control_CanBeUsedAsContentControl()
    {
        // Act
        var control = new TransitioningContentControl();
        ContentControl contentControl = control;

        // Assert
        await Assert.That(contentControl).IsNotNull();
        await Assert.That(contentControl).IsSameReferenceAs(control);
    }

    /// <summary>
    /// Test constructor is public and accessible.
    /// </summary>
    [Test]
    public async Task Constructor_IsPublicAndAccessible()
    {
        // Act & Assert
        await Assert.That(() => new TransitioningContentControl()).ThrowsNothing();
    }

    /// <summary>
    /// Test control can have content set.
    /// </summary>
    [Test]
    public async Task Control_CanHaveContentSet()
    {
        // Arrange
        var control = new TransitioningContentControl();
        var testContent = "Test Content";

        // Act
        control.Content = testContent;

        // Assert
        await Assert.That(control.Content).IsEqualTo(testContent);
    }

    /// <summary>
    /// Test control content can be null.
    /// </summary>
    [Test]
    public async Task Control_ContentCanBeNull()
    {
        // Arrange
        var control = new TransitioningContentControl();

        // Act
        control.Content = null;

        // Assert
        await Assert.That(control.Content).IsNull();
    }

    /// <summary>
    /// Test control can have UI element as content.
    /// </summary>
    [Test]
    public async Task Control_CanHaveUIElementAsContent()
    {
        // Arrange
        var control = new TransitioningContentControl();
        var button = new Button { Content = "Test Button" };

        // Act
        control.Content = button;

        // Assert
        await Assert.That(control.Content).IsEqualTo(button);
        await Assert.That(control.Content).IsAssignableTo<Button>();
    }

    /// <summary>
    /// Test control inherits ContentControl properties and methods.
    /// </summary>
    [Test]
    public async Task Control_InheritsContentControlFeatures()
    {
        // Arrange
        var control = new TransitioningContentControl();

        // Act & Assert - Test that we have access to ContentControl properties
        await Assert.That(() => control.Content).ThrowsNothing();
        await Assert.That(() => control.ContentTemplate).ThrowsNothing();
        await Assert.That(() => control.HorizontalContentAlignment).ThrowsNothing();
        await Assert.That(() => control.VerticalContentAlignment).ThrowsNothing();
    }

    /// <summary>
    /// Test control namespace is correct.
    /// </summary>
    [Test]
    public async Task Control_HasCorrectNamespace()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        await Assert.That(control.GetType().Namespace).IsEqualTo("ReactiveUI.Uno");
    }
}

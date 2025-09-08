// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Controls;

/// <summary>
/// Tests for TransitioningContentControl functionality.
/// </summary>
[TestFixture]
public class TransitioningContentControlTests
{
    /// <summary>
    /// Test constructor creates instance successfully.
    /// </summary>
    [Test]
    public void Constructor_CreatesInstanceSuccessfully()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(control, Is.Not.Null);
            Assert.That(control, Is.InstanceOf<ContentControl>());
        });
    }

    /// <summary>
    /// Test control inherits from ContentControl.
    /// </summary>
    [Test]
    public void Control_InheritsFromContentControl()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        Assert.That(control, Is.InstanceOf<ContentControl>());
    }

    /// <summary>
    /// Test control is partial class (compile-time verification).
    /// </summary>
    [Test]
    public void Control_IsPartialClass()
    {
        // This test ensures the class compiles correctly as a partial class
        var control = new TransitioningContentControl();
        Assert.That(control, Is.Not.Null);
    }

    /// <summary>
    /// Test multiple instantiation creates separate objects.
    /// </summary>
    [Test]
    public void MultipleInstantiation_CreatesSeparateObjects()
    {
        // Act
        var control1 = new TransitioningContentControl();
        var control2 = new TransitioningContentControl();

        // Assert
        Assert.That(control1, Is.Not.SameAs(control2));
    }

    /// <summary>
    /// Test control can be used as ContentControl.
    /// </summary>
    [Test]
    public void Control_CanBeUsedAsContentControl()
    {
        // Act
        var control = new TransitioningContentControl();
        ContentControl contentControl = control;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(contentControl, Is.Not.Null);
            Assert.That(contentControl, Is.SameAs(control));
        });
    }

    /// <summary>
    /// Test constructor is public and accessible.
    /// </summary>
    [Test]
    public void Constructor_IsPublicAndAccessible()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => new TransitioningContentControl());
    }

    /// <summary>
    /// Test control can have content set.
    /// </summary>
    [Test]
    public void Control_CanHaveContentSet()
    {
        // Arrange
        var control = new TransitioningContentControl();
        var testContent = "Test Content";

        // Act
        control.Content = testContent;

        // Assert
        Assert.That(control.Content, Is.EqualTo(testContent));
    }

    /// <summary>
    /// Test control content can be null.
    /// </summary>
    [Test]
    public void Control_ContentCanBeNull()
    {
        // Arrange
        var control = new TransitioningContentControl();

        // Act
        control.Content = null;

        // Assert
        Assert.That(control.Content, Is.Null);
    }

    /// <summary>
    /// Test control can have UI element as content.
    /// </summary>
    [Test]
    public void Control_CanHaveUIElementAsContent()
    {
        // Arrange
        var control = new TransitioningContentControl();
        var button = new Button { Content = "Test Button" };

        // Act
        control.Content = button;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(control.Content, Is.EqualTo(button));
            Assert.That(control.Content, Is.InstanceOf<Button>());
        });
    }

    /// <summary>
    /// Test control inherits ContentControl properties and methods.
    /// </summary>
    [Test]
    public void Control_InheritsContentControlFeatures()
    {
        // Arrange
        var control = new TransitioningContentControl();

        // Act & Assert - Test that we have access to ContentControl properties
        Assert.Multiple(() =>
        {
            Assert.That(() => control.Content, Throws.Nothing);
            Assert.That(() => control.ContentTemplate, Throws.Nothing);
            Assert.That(() => control.HorizontalContentAlignment, Throws.Nothing);
            Assert.That(() => control.VerticalContentAlignment, Throws.Nothing);
        });
    }

    /// <summary>
    /// Test control namespace is correct.
    /// </summary>
    [Test]
    public void Control_HasCorrectNamespace()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        Assert.That(control.GetType().Namespace, Is.EqualTo("ReactiveUI.Uno"));
    }
}
// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Controls;

/// <summary>Tests for TransitioningContentControl functionality.</summary>
public class TransitioningContentControlTests
{
    /// <summary>The reason UI-dependent tests are skipped in a headless environment.</summary>
    private const string NoUiContextSkipReason = "No UI context is available in the headless environment.";

    /// <summary>Setup for each test.</summary>
    [Before(Test)]
    public void SetUp()
    {
        // Skip tests if no UI context is available (headless environment)
        try
        {
            var window = Microsoft.UI.Xaml.Window.Current;
            if (window is null)
            {
                Skip.Test(NoUiContextSkipReason);
            }
        }
        catch (TypeInitializationException)
        {
            Skip.Test(NoUiContextSkipReason);
        }
        catch (NotSupportedException)
        {
            Skip.Test(NoUiContextSkipReason);
        }
    }

    /// <summary>Test constructor creates instance successfully.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Constructor_CreatesInstanceSuccessfully()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        await Assert.That(control).IsNotNull();
        await Assert.That(control).IsAssignableTo<ContentControl>();
    }

    /// <summary>Test control inherits from ContentControl.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Control_InheritsFromContentControl()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        await Assert.That(control).IsAssignableTo<ContentControl>();
    }

    /// <summary>Test control is partial class (compile-time verification).</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Control_IsPartialClass()
    {
        // This test ensures the class compiles correctly as a partial class
        var control = new TransitioningContentControl();
        await Assert.That(control).IsNotNull();
    }

    /// <summary>Test multiple instantiation creates separate objects.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task MultipleInstantiation_CreatesSeparateObjects()
    {
        // Act
        var control1 = new TransitioningContentControl();
        var control2 = new TransitioningContentControl();

        // Assert
        await Assert.That(control1).IsNotSameReferenceAs(control2);
    }

    /// <summary>Test control can be used as ContentControl.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
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

    /// <summary>Test constructor is public and accessible.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Constructor_IsPublicAndAccessible() =>
        await Assert.That(() => new TransitioningContentControl()).ThrowsNothing();

    /// <summary>Test control can have content set.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Control_CanHaveContentSet()
    {
        // Arrange
        var control = new TransitioningContentControl();
        const string testContent = "Test Content";

        // Act
        control.Content = testContent;

        // Assert
        await Assert.That(control.Content).IsEqualTo(testContent);
    }

    /// <summary>Test control content can be null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Control_ContentCanBeNull()
    {
        // Arrange
        var control = new TransitioningContentControl() { Content = null };

        // Assert
        await Assert.That(control.Content).IsNull();
    }

    /// <summary>Test control can have UI element as content.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
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

    /// <summary>Test control inherits ContentControl properties and methods.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
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

    /// <summary>Test control namespace is correct.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Control_HasCorrectNamespace()
    {
        // Act
        var control = new TransitioningContentControl();

        // Assert
        await Assert.That(control.GetType().Namespace).IsEqualTo("ReactiveUI.Uno");
    }
}

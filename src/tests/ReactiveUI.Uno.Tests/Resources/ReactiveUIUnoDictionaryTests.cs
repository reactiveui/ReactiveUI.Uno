// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Resources;

/// <summary>
/// Tests for ReactiveUIUnoDictionary functionality.
/// </summary>
public class ReactiveUIUnoDictionaryTests
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
    /// Test constructor sets Source property correctly.
    /// </summary>
    [Test]
    public async Task Constructor_SetsSourceCorrectly()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        await Assert.That(dictionary.Source).IsNotNull();
        await Assert.That(dictionary.Source!.ToString()).IsEqualTo("ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml");
        await Assert.That(dictionary.Source.IsAbsoluteUri).IsTrue();
    }

    /// <summary>
    /// Test multiple instantiation creates separate objects.
    /// </summary>
    [Test]
    public async Task MultipleInstantiation_CreatesSeparateObjects()
    {
        // Act
        var dictionary1 = new ReactiveUIUnoDictionary();
        var dictionary2 = new ReactiveUIUnoDictionary();

        // Assert
        await Assert.That(dictionary1).IsNotSameReferenceAs(dictionary2);
        await Assert.That(dictionary1.Source).IsEqualTo(dictionary2.Source);
    }

    /// <summary>
    /// Test dictionary inherits from ResourceDictionary.
    /// </summary>
    [Test]
    public async Task Dictionary_InheritsFromResourceDictionary()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        await Assert.That(dictionary).IsAssignableTo<ResourceDictionary>();
    }

    /// <summary>
    /// Test dictionary is partial class (compile-time verification).
    /// </summary>
    [Test]
    public async Task Dictionary_IsPartialClass()
    {
        // This test ensures the class compiles correctly as a partial class
        var dictionary = new ReactiveUIUnoDictionary();
        await Assert.That(dictionary).IsNotNull();
    }

    /// <summary>
    /// Test that Source URI scheme is correct.
    /// </summary>
    [Test]
    public async Task Source_HasCorrectScheme()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        await Assert.That(dictionary.Source!.Scheme).IsEqualTo("ms-appx");
    }

    /// <summary>
    /// Test that Source URI path is correct.
    /// </summary>
    [Test]
    public async Task Source_HasCorrectPath()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        await Assert.That(dictionary.Source!.AbsolutePath).IsEqualTo("/ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml");
    }

    /// <summary>
    /// Test constructor is public and accessible.
    /// </summary>
    [Test]
    public async Task Constructor_IsPublicAndAccessible()
    {
        // Act & Assert
        await Assert.That(() => new ReactiveUIUnoDictionary()).ThrowsNothing();
    }

    /// <summary>
    /// Test that the dictionary can be used as ResourceDictionary.
    /// </summary>
    [Test]
    public async Task Dictionary_CanBeUsedAsResourceDictionary()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();
        ResourceDictionary resourceDict = dictionary;

        // Assert
        await Assert.That(resourceDict).IsNotNull();
        await Assert.That(resourceDict.Source).IsEqualTo(dictionary.Source);
    }

    /// <summary>
    /// Test that Source is set during construction and not null.
    /// </summary>
    [Test]
    public async Task Source_IsNotNullAfterConstruction()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        await Assert.That(dictionary.Source).IsNotNull();
    }

    /// <summary>
    /// Test the constant dictionary URI value.
    /// </summary>
    [Test]
    public async Task DictionaryUri_HasExpectedValue()
    {
        // This test verifies the URI constant through behavior
        var dictionary = new ReactiveUIUnoDictionary();
        var expectedUri = "ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml";

        await Assert.That(dictionary.Source?.ToString()).IsEqualTo(expectedUri);
    }
}

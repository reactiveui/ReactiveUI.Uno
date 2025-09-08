// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests.Resources;

/// <summary>
/// Tests for ReactiveUIUnoDictionary functionality.
/// </summary>
[TestFixture]
public class ReactiveUIUnoDictionaryTests
{
    /// <summary>
    /// Test constructor sets Source property correctly.
    /// </summary>
    [Test]
    public void Constructor_SetsSourceCorrectly()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dictionary.Source, Is.Not.Null);
            Assert.That(dictionary.Source!.ToString(), Is.EqualTo("ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml"));
            Assert.That(dictionary.Source.IsAbsoluteUri, Is.True);
        });
    }

    /// <summary>
    /// Test multiple instantiation creates separate objects.
    /// </summary>
    [Test]
    public void MultipleInstantiation_CreatesSeparateObjects()
    {
        // Act
        var dictionary1 = new ReactiveUIUnoDictionary();
        var dictionary2 = new ReactiveUIUnoDictionary();

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(dictionary1, Is.Not.SameAs(dictionary2));
            Assert.That(dictionary1.Source, Is.EqualTo(dictionary2.Source));
        });
    }

    /// <summary>
    /// Test dictionary inherits from ResourceDictionary.
    /// </summary>
    [Test]
    public void Dictionary_InheritsFromResourceDictionary()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        Assert.That(dictionary, Is.InstanceOf<ResourceDictionary>());
    }

    /// <summary>
    /// Test dictionary is partial class (compile-time verification).
    /// </summary>
    [Test]
    public void Dictionary_IsPartialClass()
    {
        // This test ensures the class compiles correctly as a partial class
        var dictionary = new ReactiveUIUnoDictionary();
        Assert.That(dictionary, Is.Not.Null);
    }

    /// <summary>
    /// Test that Source URI scheme is correct.
    /// </summary>
    [Test]
    public void Source_HasCorrectScheme()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        Assert.That(dictionary.Source!.Scheme, Is.EqualTo("ms-appx"));
    }

    /// <summary>
    /// Test that Source URI path is correct.
    /// </summary>
    [Test]
    public void Source_HasCorrectPath()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        Assert.That(dictionary.Source!.AbsolutePath, Is.EqualTo("/ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml"));
    }

    /// <summary>
    /// Test constructor is public and accessible.
    /// </summary>
    [Test]
    public void Constructor_IsPublicAndAccessible()
    {
        // Act & Assert
        Assert.DoesNotThrow(() => new ReactiveUIUnoDictionary());
    }

    /// <summary>
    /// Test that the dictionary can be used as ResourceDictionary.
    /// </summary>
    [Test]
    public void Dictionary_CanBeUsedAsResourceDictionary()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();
        ResourceDictionary resourceDict = dictionary;

        // Assert
        Assert.Multiple(() =>
        {
            Assert.That(resourceDict, Is.Not.Null);
            Assert.That(resourceDict.Source, Is.EqualTo(dictionary.Source));
        });
    }

    /// <summary>
    /// Test that Source is set during construction and not null.
    /// </summary>
    [Test]
    public void Source_IsNotNullAfterConstruction()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        Assert.That(dictionary.Source, Is.Not.Null);
    }

    /// <summary>
    /// Test the constant dictionary URI value.
    /// </summary>
    [Test]
    public void DictionaryUri_HasExpectedValue()
    {
        // This test verifies the URI constant through behavior
        var dictionary = new ReactiveUIUnoDictionary();
        var expectedUri = "ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml";

        Assert.That(dictionary.Source?.ToString(), Is.EqualTo(expectedUri));
    }
}
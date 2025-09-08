// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using NUnit.Framework;
using System.Reflection;

namespace ReactiveUI.Uno.Tests.Resources;

/// <summary>
/// Tests for the <see cref="ReactiveUIUnoDictionary"/> class.
/// </summary>
[TestFixture]
public class ReactiveUIUnoDictionaryTests
{
    /// <summary>
    /// Verifies constructor creates proper URI.
    /// </summary>
    [Test]
    public void Constructor_CreatesProperUri()
    {
        // Act
        var dictionary = new ReactiveUIUnoDictionary();

        // Assert
        Assert.That(dictionary.Source, Is.Not.Null);
        Assert.That(dictionary.Source.ToString(), Is.EqualTo("ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml"));
        Assert.That(dictionary.Source.IsAbsoluteUri, Is.True);
        Assert.That(dictionary.Source.Scheme, Is.EqualTo("ms-appx"));
    }

    /// <summary>
    /// Verifies the dictionary URI constant is correct.
    /// </summary>
    [Test]
    public void DictionaryUri_IsCorrect()
    {
        // Use reflection to access the private constant
        var field = typeof(ReactiveUIUnoDictionary).GetField("DictionaryUri", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.That(field, Is.Not.Null);
        
        var value = field!.GetValue(null) as string;
        Assert.That(value, Is.EqualTo("ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml"));
    }

    /// <summary>
    /// Verifies constructor doesn't throw.
    /// </summary>
    [Test]
    public void Constructor_DoesNotThrow()
    {
        // Act & Assert
        Assert.That(() => new ReactiveUIUnoDictionary(), Throws.Nothing);
    }
}
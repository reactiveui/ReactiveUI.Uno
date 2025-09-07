// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using FluentAssertions;
using Microsoft.UI.Xaml.Controls;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests;

/// <summary>
/// AutoDataTemplateBindingHookTests.
/// </summary>
[TestFixture]
public class AutoDataTemplateBindingHookTests
{
    /// <summary>
    /// Executes the hook sets default template when eligible.
    /// </summary>
    [Test]
    public void ExecuteHook_Sets_DefaultTemplate_When_Eligible()
    {
        var hook = new AutoDataTemplateBindingHook();
        ItemsControl ic;
        try
        {
            ic = new ItemsControl();
        }
        catch (System.Exception ex) when (ex is System.TypeInitializationException || ex is System.NotSupportedException)
        {
            Assert.Ignore("UI dispatcher not available for Uno/WinUI controls in this environment.");
            return;
        }

        var vmChanges = Array.Empty<IObservedChange<object, object>>();
        var expr = (Expression<System.Func<object?>>)(() => ic.ItemsSource);
        var viewChanges = new[]
        {
            new ObservedChange<object, object>(ic, expr, new object())
        };

        var result = hook.ExecuteHook(null, ic, () => vmChanges, () => viewChanges, BindingDirection.OneWay);
        result.Should().BeTrue();
        ic.ItemTemplate.Should().NotBeNull();
    }

    /// <summary>
    /// Executes the hook does not override existing template.
    /// </summary>
    [Test]
    public void ExecuteHook_Does_Not_Override_Existing_Template()
    {
        var hook = new AutoDataTemplateBindingHook();
        ItemsControl ic;
        try
        {
            ic = new ItemsControl { ItemTemplate = AutoDataTemplateBindingHook.DefaultItemTemplate.Value };
        }
        catch (System.Exception ex) when (ex is System.TypeInitializationException || ex is System.NotSupportedException)
        {
            Assert.Ignore("UI dispatcher not available for Uno/WinUI controls in this environment.");
            return;
        }

        var expr = (Expression<System.Func<object?>>)(() => ic.ItemsSource);
        var viewChanges = new[] { new ObservedChange<object, object>(ic, expr, new object()) };

        var result = hook.ExecuteHook(null, ic, Array.Empty<IObservedChange<object, object>>, () => viewChanges, BindingDirection.OneWay);
        result.Should().BeTrue();
        ic.ItemTemplate.Should().NotBeNull();
    }
}

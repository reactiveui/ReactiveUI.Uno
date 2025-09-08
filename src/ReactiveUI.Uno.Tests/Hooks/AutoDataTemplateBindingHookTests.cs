// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Microsoft.UI.Xaml.Controls;
using NUnit.Framework;

namespace ReactiveUI.Uno.Tests;

/// <summary>
/// Contains test cases for the AutoDataTemplateBindingHook class,
/// ensuring its behavior adheres to expected functionality.
/// </summary>
[TestFixture]
public class AutoDataTemplateBindingHookTests
{
    /// <summary>
    /// Executes the hook to set the default template when eligible.
    /// </summary>
    [Test]
    public void ExecuteHook_Sets_DefaultTemplate_When_Eligible()
    {
        AutoDataTemplateBindingHook hook = new();
        ItemsControl ic;
        try
        {
            ic = new();
        }
        catch (Exception ex) when (ex is TypeInitializationException or NotSupportedException)
        {
            Assert.Ignore("UI dispatcher not available for Uno/WinUI controls in this environment.");
            return;
        }

        IObservedChange<object, object>[] vmChanges = [];
        var expr = (Expression<Func<object?>>)(() => ic.ItemsSource);
        ObservedChange<object, object>[] viewChanges =
        [
            new(ic, expr, new())
        ];

        var result = hook.ExecuteHook(null, ic, () => vmChanges, () => viewChanges, BindingDirection.OneWay);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(ic.ItemTemplate, Is.Not.Null);
        }
    }

    /// <summary>
    /// Executes the hook to ensure that an existing template is not overridden.
    /// </summary>
    [Test]
    public void ExecuteHook_Does_Not_Override_Existing_Template()
    {
        AutoDataTemplateBindingHook hook = new();
        ItemsControl ic;
        try
        {
            ic = new() { ItemTemplate = AutoDataTemplateBindingHook.DefaultItemTemplate.Value };
        }
        catch (Exception ex) when (ex is TypeInitializationException or NotSupportedException)
        {
            Assert.Ignore("UI dispatcher not available for Uno/WinUI controls in this environment.");
            return;
        }

        var expr = (Expression<Func<object?>>)(() => ic.ItemsSource);
        ObservedChange<object, object>[] viewChanges = [new(ic, expr, new object())];

        var result = hook.ExecuteHook(null, ic, Array.Empty<IObservedChange<object, object>>, () => viewChanges, BindingDirection.OneWay);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.True);
            Assert.That(ic.ItemTemplate, Is.Not.Null);
        }
    }
}

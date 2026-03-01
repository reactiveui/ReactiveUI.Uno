// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Microsoft.UI.Xaml.Controls;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests;

/// <summary>
/// Contains test cases for the AutoDataTemplateBindingHook class,
/// ensuring its behavior adheres to expected functionality.
/// </summary>
public class AutoDataTemplateBindingHookTests
{
    /// <summary>
    /// Executes the hook to set the default template when eligible.
    /// </summary>
    [Test]
    public async Task ExecuteHook_Sets_DefaultTemplate_When_Eligible()
    {
        AutoDataTemplateBindingHook hook = new();
        ItemsControl ic;
        try
        {
            ic = new();
        }
        catch (Exception ex) when (ex is TypeInitializationException or NotSupportedException)
        {
            Skip.Test("UI dispatcher not available for Uno/WinUI controls in this environment.");
            return;
        }

        IObservedChange<object, object>[] vmChanges = [];
        var expr = (Expression<Func<object?>>)(() => ic.ItemsSource);
        ObservedChange<object, object>[] viewChanges =
        [
            new(ic, expr, new())
        ];

        var result = hook.ExecuteHook(null, ic, () => vmChanges, () => viewChanges, BindingDirection.OneWay);
        await Assert.That(result).IsTrue();
        await Assert.That(ic.ItemTemplate).IsNotNull();
    }

    /// <summary>
    /// Executes the hook to ensure that an existing template is not overridden.
    /// </summary>
    [Test]
    public async Task ExecuteHook_Does_Not_Override_Existing_Template()
    {
        AutoDataTemplateBindingHook hook = new();
        ItemsControl ic;
        try
        {
            ic = new() { ItemTemplate = AutoDataTemplateBindingHook.DefaultItemTemplate.Value };
        }
        catch (Exception ex) when (ex is TypeInitializationException or NotSupportedException)
        {
            Skip.Test("UI dispatcher not available for Uno/WinUI controls in this environment.");
            return;
        }

        var expr = (Expression<Func<object?>>)(() => ic.ItemsSource);
        ObservedChange<object, object>[] viewChanges = [new(ic, expr, new object())];

        var result = hook.ExecuteHook(null, ic, Array.Empty<IObservedChange<object, object>>, () => viewChanges, BindingDirection.OneWay);
        await Assert.That(result).IsTrue();
        await Assert.That(ic.ItemTemplate).IsNotNull();
    }
}

// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Microsoft.UI.Xaml.Controls;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests;

/// <summary>Tests the behavior of <see cref="AutoDataTemplateBindingHook"/>.</summary>
public class AutoDataTemplateBindingHookTests
{
    /// <summary>Validates that ExecuteHook throws when getCurrentViewProperties is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task ExecuteHook_ThrowsArgumentNullException_WhenGetCurrentViewPropertiesIsNull()
    {
        AutoDataTemplateBindingHook hook = new();

        var exception = await Assert.That(
            () => hook.ExecuteHook(null!, new(), () => [], null!, BindingDirection.OneWay))
            .Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("getCurrentViewProperties");
    }

    /// <summary>Validates that ExecuteHook returns true when sender is not an ItemsControl.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task ExecuteHook_ReturnsTrue_WhenSenderIsNotItemsControl()
    {
        AutoDataTemplateBindingHook hook = new();
        var sender = new object();
        var expr = (Expression<Func<object?>>)(() => sender);
        IObservedChange<object, object>[] viewChanges = [new ObservedChange<object, object>(sender, expr, new())];

        var result = hook.ExecuteHook(null, new(), () => [], () => viewChanges, BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Executes the hook to set the default template when eligible.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
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

        IObservedChange<object, object>[] viewModelChanges = [];
        var expr = (Expression<Func<object?>>)(() => ic.ItemsSource);
        IObservedChange<object, object>[] viewChanges =
        [
            new ObservedChange<object, object>(ic, expr, new())
        ];

        var result = hook.ExecuteHook(null, ic, () => viewModelChanges, () => viewChanges, BindingDirection.OneWay);
        await Assert.That(result).IsTrue();
        await Assert.That(ic.ItemTemplate).IsNotNull();
    }

    /// <summary>Executes the hook to ensure that an existing template is not overridden.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
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
        IObservedChange<object, object>[] viewChanges = [new ObservedChange<object, object>(ic, expr, new object())];

        var result = hook.ExecuteHook(
            null,
            ic,
            Array.Empty<IObservedChange<object, object>>,
            () => viewChanges,
            BindingDirection.OneWay);
        await Assert.That(result).IsTrue();
        await Assert.That(ic.ItemTemplate).IsNotNull();
    }
}

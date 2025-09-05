// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using FluentAssertions;
using ReactiveUI;
using Xunit;
#if HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace ReactiveUI.Uno.Tests.Hooks;

public class AutoDataTemplateBindingHookTests
{
    private class TestViewModel
    {
    }

    [Fact]
    public void Sets_default_item_template_when_binding_items_source()
    {
        // Arrange
        var hook = new ReactiveUI.Uno.AutoDataTemplateBindingHook();
        var items = new ItemsControl();

        // Build a member access expression body: x => x.ItemsSource
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(ItemsControl), "x");
        var body = System.Linq.Expressions.Expression.Property(parameter, nameof(ItemsControl.ItemsSource));

        var change = new ObservedChange<object, object>(items, body, default!);

        // Act
        var ok = hook.ExecuteHook(null, items,
            () => System.Array.Empty<IObservedChange<object, object>>(),
            () => new[] { change },
            default);

        // Assert
        ok.Should().BeTrue();
        items.ItemTemplate.Should().NotBeNull();
    }
}

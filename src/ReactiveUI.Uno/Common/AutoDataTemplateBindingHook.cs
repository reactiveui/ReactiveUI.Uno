// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml.Markup;

namespace ReactiveUI.Uno;

/// <summary>
/// AutoDataTemplateBindingHook is a binding hook that checks ItemsControls
/// that don't have DataTemplates, and assigns a default DataTemplate that
/// loads the View associated with each ViewModel.
/// </summary>
public class AutoDataTemplateBindingHook : IPropertyBindingHook
{
    /// <summary>
    /// Gets the default item template.
    /// </summary>
    public static Lazy<DataTemplate> DefaultItemTemplate { get; } = new(() =>
    {
        const string template =
"""
<DataTemplate
    xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
    xmlns:rxui='using:ReactiveUI.Uno'>
    <rxui:ViewModelViewHost ViewModel="{Binding}" VerticalContentAlignment="Stretch" HorizontalContentAlignment="Stretch" IsTabStop="False" />
</DataTemplate>
""";
        return (DataTemplate)XamlReader.Load(template);
    });

    /// <inheritdoc/>
    public bool ExecuteHook(object? source, object target, Func<IObservedChange<object, object>[]> getCurrentViewModelProperties, Func<IObservedChange<object, object>[]> getCurrentViewProperties, BindingDirection direction)
    {
        ArgumentNullException.ThrowIfNull(getCurrentViewProperties);

        var viewProperties = getCurrentViewProperties();
        var lastViewProperty = viewProperties.LastOrDefault();

        if (lastViewProperty?.Sender is not ItemsControl itemsControl)
        {
            return true;
        }

        if (!string.IsNullOrEmpty(itemsControl.DisplayMemberPath))
        {
            return true;
        }

        if (viewProperties[^1].GetPropertyName() != "ItemsSource")
        {
            return true;
        }

        if (itemsControl.ItemTemplate is not null)
        {
            return true;
        }

        if (itemsControl.ItemTemplateSelector is not null)
        {
            return true;
        }

        itemsControl.ItemTemplate = DefaultItemTemplate.Value;
        return true;
    }
}

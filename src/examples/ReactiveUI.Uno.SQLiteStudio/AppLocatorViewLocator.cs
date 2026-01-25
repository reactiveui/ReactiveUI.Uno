// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.SQLiteStudio;

/// <summary>
/// A view locator that uses AppLocator to resolve views.
/// </summary>
public sealed class AppLocatorViewLocator : IViewLocator
{
    /// <inheritdoc/>
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        if (viewModel is null)
        {
            return null;
        }

        return AppLocator.Current.GetService(typeof(IViewFor<>).MakeGenericType(viewModel.GetType())) as IViewFor;
    }

    /// <inheritdoc/>
    public IViewFor<TViewModel>? ResolveView<TViewModel>(string? contract = null)
        where TViewModel : class
    {
        return AppLocator.Current.GetService<IViewFor<TViewModel>>();
    }

    /// <inheritdoc/>
    public IViewFor? ResolveView(object? viewModel, string? contract = null)
    {
        if (viewModel is null)
        {
            return null;
        }

        return AppLocator.Current.GetService(typeof(IViewFor<>).MakeGenericType(viewModel.GetType())) as IViewFor;
    }
}

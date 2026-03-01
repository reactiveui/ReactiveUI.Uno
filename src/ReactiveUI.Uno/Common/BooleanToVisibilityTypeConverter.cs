// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Uno;

/// <summary>
/// Converts between a <see cref="bool"/> and XAML <see cref="Visibility"/>.
/// to invert the result.
/// </summary>
public class BooleanToVisibilityTypeConverter : BindingTypeConverter<bool, Visibility>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(bool from, object? conversionHint, [NotNullWhen(true)] out Visibility result)
    {
        var hint = conversionHint is BooleanToVisibilityHint visibilityHint
            ? visibilityHint
            : BooleanToVisibilityHint.None;

        var value = (hint & BooleanToVisibilityHint.Inverse) != 0 ? !from : from;
        const Visibility notVisible = Visibility.Collapsed;

        result = value ? Visibility.Visible : notVisible;
        return true;
    }
}

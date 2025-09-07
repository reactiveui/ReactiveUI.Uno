// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno;

/// <summary>
/// Converts between a <see cref="bool"/> and XAML <see cref="Visibility"/>.
/// to invert the result.
/// </summary>
public class BooleanToVisibilityTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(bool) && toType == typeof(Visibility))
        {
            return 10;
        }

        if (fromType == typeof(Visibility) && toType == typeof(bool))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        var hint = conversionHint is BooleanToVisibilityHint visibilityHint ?
            visibilityHint :
            BooleanToVisibilityHint.None;

        if (toType == typeof(Visibility) && from is bool fromBool)
        {
            var fromAsBool = (hint & BooleanToVisibilityHint.Inverse) != 0 ? !fromBool : fromBool;
            result = fromAsBool ? Visibility.Visible : Visibility.Collapsed;
            return true;
        }

        if (from is Visibility fromAsVis)
        {
            // Convert Visibility back to bool, honoring the Inverse flag.
            var visible = fromAsVis == Visibility.Visible;
            var inverted = (hint & BooleanToVisibilityHint.Inverse) != 0;
            result = inverted ? !visible : visible;
            return true;
        }

        // Fallback – provide a sensible default for requested type.
        result = toType == typeof(Visibility) ? Visibility.Visible : false;
        return true;
    }
}

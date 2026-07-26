// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Converts between a <see cref="bool"/> and XAML <see cref="Visibility"/>. to invert the result.</summary>
public class BooleanToVisibilityTypeConverter : BindingTypeConverter<bool, Visibility>
{
    /// <summary>The affinity assigned to Boolean-to-visibility conversions.</summary>
    private const int ConversionAffinity = 2;

    /// <inheritdoc/>
    public override int GetAffinityForObjects() => ConversionAffinity;

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

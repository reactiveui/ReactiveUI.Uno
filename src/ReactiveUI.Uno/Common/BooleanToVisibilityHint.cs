// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Hints to control how <see cref="bool"/> to <see cref="Visibility"/> conversion behaves.</summary>
[Flags]
public enum BooleanToVisibilityHint
{
    /// <summary>Uses the normal true-visible and false-collapsed mapping.</summary>
    None = 0,

    /// <summary>Inverts the true-visible and false-collapsed mapping.</summary>
    Inverse = 1 << 1,
}

// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Represents the ReactiveUI Uno controls resource dictionary.</summary>
/// <seealso cref="ResourceDictionary" />
public class ReactiveUIUnoDictionary : ResourceDictionary
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveUIUnoDictionary"/> class.</summary>
    [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(ViewModelViewHost))]
    public ReactiveUIUnoDictionary()
    {
        // Preserve the XAML control without constructing an unused view or resolving services during startup.
    }
}

// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Represents the ReactiveUI Uno controls resource dictionary.</summary>
/// <seealso cref="ResourceDictionary" />
public class ReactiveUIUnoDictionary : ResourceDictionary
{
    /// <summary>Resource URI for the ReactiveUI Uno dictionary.</summary>
#if REACTIVE_SHIM
    private const string DictionaryUri = "ms-appx:///ReactiveUI.Uno.Reactive/Resources/ReactiveUI.Uno.Reactive.xaml";
#else
    private const string DictionaryUri = "ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml";
#endif

    /// <summary>Initializes a new instance of the <see cref="ReactiveUIUnoDictionary"/> class.</summary>
    public ReactiveUIUnoDictionary() => Source = new(DictionaryUri, UriKind.Absolute);
}

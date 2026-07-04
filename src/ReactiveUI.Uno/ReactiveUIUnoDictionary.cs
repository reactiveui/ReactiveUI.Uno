// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
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

    /// <summary>Initializes a new instance of the <see cref="ReactiveUIUnoDictionary"/> class. Sets the Source to the embedded ResourceDictionary.</summary>
    public ReactiveUIUnoDictionary() => Source = new(DictionaryUri, UriKind.Absolute);
}

// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno;

/// <summary>
/// ReactiveUIUnoControlsDictionary.
/// </summary>
/// <seealso cref="ResourceDictionary" />
public partial class ReactiveUIUnoDictionary : ResourceDictionary
{
    private const string DictionaryUri = "ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml";

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUIUnoDictionary"/> class.
    /// Sets the Source to the embedded ResourceDictionary.
    /// </summary>
    public ReactiveUIUnoDictionary() => Source = new Uri(DictionaryUri, UriKind.Absolute);
}

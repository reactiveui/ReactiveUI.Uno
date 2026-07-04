// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
#if !WINDOWS
using System.Windows.Markup;
#endif

#if !WINDOWS
#if REACTIVE_SHIM
[assembly: XmlnsDefinition("http://reactiveui.net", "ReactiveUI.Uno.Reactive")]
#else
[assembly: XmlnsDefinition("http://reactiveui.net", "ReactiveUI.Uno")]
#endif
#endif
[assembly: InternalsVisibleTo("ReactiveUI.Uno.Tests")]
#if REACTIVE_SHIM
[assembly: InternalsVisibleTo("ReactiveUI.Uno.Reactive.Tests")]
#endif

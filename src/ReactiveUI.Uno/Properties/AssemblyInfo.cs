// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
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

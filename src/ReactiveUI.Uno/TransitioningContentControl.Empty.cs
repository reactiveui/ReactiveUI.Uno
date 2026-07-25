// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>A ContentControl that displays its content with animated transitions when the content changes.</summary>
/// <remarks>Use TransitioningContentControl to provide visual transitions between different pieces of content,
/// such as when switching views or updating displayed data. The specific transition effect may depend on the control's
/// configuration or style. This control is typically used to enhance user experience by animating content changes in
/// user interfaces.</remarks>
public class TransitioningContentControl : ContentControl;

﻿// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if HAS_WINUI
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml.Controls;
#endif

namespace ReactiveUI.Uno;

/// <summary>
/// A control with a single transition.
/// </summary>
public partial class TransitioningContentControl : ContentControl;

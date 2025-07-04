﻿// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

#if HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace ReactiveUI.Uno;

/// <summary>
/// A <see cref="UserControl"/> that is reactive.
/// </summary>
/// <remarks>
/// <para>
/// This class is a <see cref="UserControl"/> that is also reactive. That is, it implements <see cref="IViewFor{TViewModel}"/>.
/// You can extend this class to get an implementation of <see cref="IViewFor{TViewModel}"/> rather than writing one yourself.
/// </para>
/// <para>
/// Note that the XAML for your control must specify the same base class, including the generic argument you provide for your view
/// model. To do this, use the <c>TypeArguments</c> attribute as follows:
/// <code>
/// <![CDATA[
/// <rxui:ReactiveUserControl
///         x:Class="Foo.Bar.Views.YourView"
///         x:TypeArguments="vms:YourViewModel"
///         xmlns:rxui="http://reactiveui.net"
///         xmlns:vms="clr-namespace:Foo.Bar.ViewModels"
///         xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
///         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
///         xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
///         xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
///         mc:Ignorable="d">
///     <!-- view XAML here -->
/// </rxui:ReactiveUserControl>
/// ]]>
/// </code>
/// </para>
/// <para>
/// Note that UWP and WinUI projects do not support the <c>TypeArguments</c> attribute. The XAML designer window in WPF projects also does not
/// support generic types. To use <see cref="ReactiveUserControl{TViewModel}"/> in XAML documents you need to create a base class
/// where you derive from <see cref="ReactiveUserControl{TViewModel}"/> with the type argument filled in.
/// <code>
/// <![CDATA[
/// internal class YourViewBase : ReactiveUserControl<YourViewModel> { /* No code needed here */ }
///
/// public partial class YourView : YourViewBase
/// {
///     /* Your code */
/// }
/// ]]>
/// </code>
/// Then you can use this base class as root in your XAML document.
/// <code>
/// <![CDATA[
/// <views:YourViewBase
///         x:Class="Foo.Bar.Views.YourView"
///         xmlns:rxui="http://reactiveui.net"
///         xmlns:vms="clr-namespace:Foo.Bar.ViewModels"
///         xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
///         xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
///         xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
///         xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
///         mc:Ignorable="d">
///     <!-- view XAML here -->
/// </views:YourViewBase>
/// ]]>
/// </code>
/// </para>
/// </remarks>
/// <typeparam name="TViewModel">
/// The type of the view model backing the view.
/// </typeparam>
#if IOS
[global::Foundation.Register]
#endif
public partial class ReactiveUserControl<TViewModel> :
        UserControl, IViewFor<TViewModel>
        where TViewModel : class
{
    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ReactiveUserControl<TViewModel>),
            new PropertyMetadata(null));

    static ReactiveUserControl() => _ = ActivationHelper.UnoActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUserControl{TViewModel}"/> class.
    /// </summary>
    protected ReactiveUserControl()
    {
        // needed so the others are optional.
    }

#if ANDROID
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUserControl{TViewModel}"/> class.
    /// Native constructor, do not use explicitly.
    /// </summary>
    /// <remarks>
    /// Used by the Xamarin Runtime to materialize native
    /// objects that may have been collected in the managed world.
    /// </remarks>
    /// <param name="javaReference">A <see cref="IntPtr"/> containing a Java Native Interface (JNI) object reference.</param>
    /// <param name="transfer">A <see cref="JniHandleOwnership"/> indicating how to handle handle.</param>
    protected ReactiveUserControl(IntPtr javaReference, global::Android.Runtime.JniHandleOwnership transfer)
        : base(javaReference, transfer)
    {
    }
#endif
#if IOS
    /// <summary>
    /// Initializes a new instance of the <see cref="ReactiveUserControl{TViewModel}"/> class.
    /// Native constructor, do not use explicitly.
    /// </summary>
    /// <param name="handle">Handle to the native control.</param>
    /// <remarks>
    /// Used by the Xamarin Runtime to materialize native.
    /// objects that may have been collected in the managed world.
    /// </remarks>
    protected ReactiveUserControl(IntPtr handle)
        : base(handle)
    {
    }
#endif

    /// <summary>
    /// Gets the binding root view model.
    /// </summary>
    public TViewModel? BindingRoot => ViewModel;

    /// <inheritdoc/>
    public TViewModel? ViewModel
    {
        get => (TViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <inheritdoc/>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = (TViewModel?)value;
    }
}

﻿// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using Splat;

#if HAS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace ReactiveUI.Uno;

/// <summary>
/// This content control will automatically load the View associated with
/// the ViewModel property and display it. This control is very useful
/// inside a DataTemplate to display the View associated with a ViewModel.
/// </summary>
public partial class ViewModelViewHost : TransitioningContentControl, IViewFor, IEnableLogger
{
    /// <summary>
    /// The default content dependency property.
    /// </summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

    /// <summary>
    /// The view model dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

    /// <summary>
    /// The view contract observable dependency property.
    /// </summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(ViewModelViewHost), new PropertyMetadata(Observable<string>.Default));

    private string? _viewContract;

    static ViewModelViewHost() => _ = ActivationHelper.UnoActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
    /// </summary>
    public ViewModelViewHost()
    {
        var platform = Locator.Current.GetService<IPlatformOperations>();
        Func<string?> platformGetter = () => default;

        if (platform is null)
        {
            // NB: This used to be an error but WPF design mode can't read
            // good or do other stuff good.
            this.Log().Error("Couldn't find an IPlatformOperations implementation. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");
        }
        else
        {
            platformGetter = () => platform.GetOrientation();
        }

        ViewContractObservable = ModeDetector.InUnitTestRunner()
            ? Observable<string?>.Never
            : Observable.FromEvent<SizeChangedEventHandler, string?>(
              eventHandler =>
              {
                  void Handler(object? sender, SizeChangedEventArgs e) => eventHandler(platformGetter());
                  return Handler;
              },
              x => SizeChanged += x,
              x => SizeChanged -= x)
              .StartWith(platformGetter())
              .DistinctUntilChanged();

        var contractChanged = this.WhenAnyObservable(x => x.ViewContractObservable).Do(x => _viewContract = x).StartWith(ViewContract);
        var viewModelChanged = this.WhenAnyValue(x => x.ViewModel).StartWith(ViewModel);
        var vmAndContract = contractChanged
            .CombineLatest(viewModelChanged, (contract, vm) => (ViewModel: vm, Contract: contract));

        this.WhenActivated(d =>
        {
            d(contractChanged
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(x => _viewContract = x ?? string.Empty));

            d(vmAndContract.DistinctUntilChanged().Subscribe(x => ResolveViewForViewModel(x.ViewModel, x.Contract)));
        });
    }

    /// <summary>
    /// Gets or sets the view contract observable.
    /// </summary>
    public IObservable<string?> ViewContractObservable
    {
        get => (IObservable<string>)GetValue(ViewContractObservableProperty);
        set => SetValue(ViewContractObservableProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed by default when no content is set.
    /// </summary>
    public object DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the ViewModel to display.
    /// </summary>
    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>
    /// Gets or sets the view contract.
    /// </summary>
    public string? ViewContract
    {
        get => _viewContract;
        set => ViewContractObservable = Observable.Return(value);
    }

    /// <summary>
    /// Gets or sets the view locator.
    /// </summary>
    public IViewLocator? ViewLocator { get; set; }

    private void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
        var viewInstance = viewLocator.ResolveView(viewModel, contract) ?? viewLocator.ResolveView(viewModel);

        if (viewInstance is null)
        {
            Content = DefaultContent;
            this.Log().Warn($"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}.");
            return;
        }

        viewInstance.ViewModel = viewModel;

        Content = viewInstance;
    }
}

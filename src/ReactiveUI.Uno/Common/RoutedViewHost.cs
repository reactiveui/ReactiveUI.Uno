// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

#if HAS_WINUI
using Microsoft.UI.Xaml;
#else
using Windows.UI.Xaml;
#endif

namespace ReactiveUI.Uno;

/// <summary>
/// This control hosts the View associated with a Router, and will display
/// the View and wire up the ViewModel whenever a new ViewModel is
/// navigated to. Put this control as the only control in your Window.
/// </summary>
public partial class RoutedViewHost : TransitioningContentControl, IActivatableView, IEnableLogger
{
    /// <summary>
    /// The router dependency property.
    /// </summary>
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register(nameof(Router), typeof(RoutingState), typeof(RoutedViewHost), new PropertyMetadata(null));

    /// <summary>
    /// The default content property.
    /// </summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(RoutedViewHost), new PropertyMetadata(null));

    /// <summary>
    /// The view contract observable property.
    /// </summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string?>), typeof(RoutedViewHost), new PropertyMetadata(Observable.Never<string?>()));

    private string? _viewContract;

    static RoutedViewHost() => _ = ActivationHelper.UnoActivated;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedViewHost"/> class.
    /// </summary>
    public RoutedViewHost()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        var platform = Locator.Current.GetService<IPlatformOperations>();
        Func<string?> platformGetter = () => default;

        if (platform is null)
        {
            this.Log().Error("Couldn't find an IPlatformOperations implementation. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");
        }
        else
        {
            platformGetter = () => platform.GetOrientation();
        }

        ViewContractObservable = ModeDetector.InUnitTestRunner()
            ? Observable.Never<string?>()
            : Observable.FromEvent<SizeChangedEventHandler, string?>(
                eventHandler =>
                {
                    void Handler(object sender, SizeChangedEventArgs e) => eventHandler(platformGetter());
                    return Handler;
                },
                x => SizeChanged += x,
                x => SizeChanged -= x)
           .StartWith(platformGetter())
           .DistinctUntilChanged();

        IRoutableViewModel? currentViewModel = null;
        var vmAndContract = this.WhenAnyObservable(x => x.Router.CurrentViewModel).Do(x => currentViewModel = x).StartWith(currentViewModel).CombineLatest(
            this.WhenAnyObservable(x => x.ViewContractObservable).Do(x => _viewContract = x).StartWith(ViewContract),
            (viewModel, contract) => (viewModel, contract));

        if (ModeDetector.InUnitTestRunner())
        {
            vmAndContract
                .DistinctUntilChanged()
                .Subscribe(ResolveViewForViewModel, ex => RxApp.DefaultExceptionHandler.OnNext(ex));
            return;
        }

        this.WhenActivated(d =>
            d(vmAndContract.DistinctUntilChanged<(IRoutableViewModel? viewModel, string? contract)>().Subscribe(
                ResolveViewForViewModel,
                ex => RxApp.DefaultExceptionHandler.OnNext(ex))));
    }

    /// <summary>
    /// Gets or sets the <see cref="RoutingState"/> of the view model stack.
    /// </summary>
    public RoutingState Router
    {
        get => (RoutingState)GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    /// <summary>
    /// Gets or sets the content displayed whenever there is no page currently
    /// routed.
    /// </summary>
    public object DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the view contract observable.
    /// </summary>
    /// <value>
    /// The view contract observable.
    /// </value>
    public IObservable<string?> ViewContractObservable
    {
        get => (IObservable<string?>)GetValue(ViewContractObservableProperty);
        set => SetValue(ViewContractObservableProperty, value);
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
    /// <value>
    /// The view locator.
    /// </value>
    public IViewLocator? ViewLocator { get; set; }

    private void ResolveViewForViewModel((IRoutableViewModel? viewModel, string? contract) x)
    {
        if (x.viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
        var view = (viewLocator.ResolveView(x.viewModel, x.contract) ?? viewLocator.ResolveView(x.viewModel)) ?? throw new Exception($"Couldn't find view for '{x.viewModel}'.");
        view.ViewModel = x.viewModel;
        Content = view;
    }
}

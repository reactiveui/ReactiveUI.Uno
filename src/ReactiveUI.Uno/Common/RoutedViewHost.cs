// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace ReactiveUI.Uno;

/// <summary>
/// This control hosts the View associated with a Router, and will display
/// the View and wire up the ViewModel whenever a new ViewModel is
/// navigated to. Put this control as the only control in your Window.
/// </summary>
[RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
public partial class RoutedViewHost : TransitioningContentControl, IActivatableView, IEnableLogger
{
    /// <summary>
    /// Identifies the Router dependency property, which holds the current routing state for the view host.
    /// </summary>
    /// <remarks>This field is used when interacting with the WPF property system, such as when calling
    /// methods like SetValue or GetValue on a RoutedViewHost instance. The Router property enables data binding,
    /// styling, and animation support for routing state within the view host.</remarks>
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register(nameof(Router), typeof(RoutingState), typeof(RoutedViewHost), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the DefaultContent dependency property.
    /// </summary>
    /// <remarks>This field is used to register and reference the DefaultContent property with the Windows
    /// Presentation Foundation (WPF) property system. It is typically used when calling methods such as SetValue or
    /// GetValue on instances of RoutedViewHost.</remarks>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(RoutedViewHost), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the ViewContractObservable dependency property.
    /// </summary>
    /// <remarks>This field is used to register and reference the ViewContractObservable property with the WPF
    /// property system. It enables data binding, styling, and other WPF property features for the
    /// ViewContractObservable property on RoutedViewHost instances.</remarks>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string?>), typeof(RoutedViewHost), new PropertyMetadata(Observable.Never<string?>()));

    private string? _viewContract;

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
                .Subscribe(ResolveViewForViewModel, ex => RxState.DefaultExceptionHandler.OnNext(ex));
            return;
        }

        this.WhenActivated(d =>
            d(vmAndContract.DistinctUntilChanged().Subscribe(
                ResolveViewForViewModel,
                ex => RxState.DefaultExceptionHandler.OnNext(ex))));
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

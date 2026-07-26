// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI;
using Splat;
#if REACTIVE_SHIM
using ReactiveUI.Uno.Reactive.Internal;
#else
using ReactiveUI.Uno.Internal;
#endif

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>
/// This control hosts the View associated with a Router, and will display
/// the View and wire up the ViewModel whenever a new ViewModel is
/// navigated to. Put this control as the only control in your Window.
/// </summary>
[RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
public class RoutedViewHost : TransitioningContentControl, IActivatableView, IEnableLogger
{
    /// <summary>Identifies the dependency property that holds the current routing state.</summary>
    /// <remarks>This field is used when interacting with the WPF property system, such as when calling
    /// methods like SetValue or GetValue on a RoutedViewHost instance. The Router property enables data binding,
    /// styling, and animation support for routing state within the view host.</remarks>
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register(
            nameof(Router),
            typeof(RoutingState),
            typeof(RoutedViewHost),
            new(null));

    /// <summary>Identifies the DefaultContent dependency property.</summary>
    /// <remarks>This field is used to register and reference the DefaultContent property with the Windows
    /// Presentation Foundation (WPF) property system. It is typically used when calling methods such as SetValue or
    /// GetValue on instances of RoutedViewHost.</remarks>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(
            nameof(DefaultContent),
            typeof(object),
            typeof(RoutedViewHost),
            new(null));

    /// <summary>Identifies the ViewContractObservable dependency property.</summary>
    /// <remarks>This field is used to register and reference the ViewContractObservable property with the WPF
    /// property system. It enables data binding, styling, and other WPF property features for the
    /// ViewContractObservable property on RoutedViewHost instances.</remarks>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(
            nameof(ViewContractObservable),
            typeof(IObservable<string?>),
            typeof(RoutedViewHost),
            new(Observable.Never<string?>()));

    /// <summary>Stores the latest resolved view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class.</summary>
    public RoutedViewHost()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        var platform = Locator.Current.GetService<IPlatformOperations>();
        Func<string?> platformGetter = static () => default;

        if (platform is null)
        {
            this.Log().Error(
                string.Concat(
                    "Couldn't find an IPlatformOperations implementation. ",
                    "Please make sure you have installed the latest version of the ReactiveUI packages ",
                    "for your platform. ",
                    "See https://reactiveui.net/docs/getting-started/installation for guidance."));
        }
        else
        {
            platformGetter = () => platform.GetOrientation();
        }

        Action<SizeChangedEventHandler> addSizeChangedHandler = handler => SizeChanged += handler;
        Action<SizeChangedEventHandler> removeSizeChangedHandler = handler => SizeChanged -= handler;

        ViewContractObservable = ModeDetector.InUnitTestRunner()
            ? Observable.Never<string?>()
            : CreateViewContractObservable(addSizeChangedHandler, removeSizeChangedHandler, platformGetter)
           .StartWith(platformGetter())
           .DistinctUntilChanged();

        IRoutableViewModel? currentViewModel = null;
        var currentViewModelChanged = this
            .WhenAnyObservable(x => x.Router.CurrentViewModel)
            .Do(x => currentViewModel = x)
            .StartWith(currentViewModel);
        var viewContractChanged = this
            .WhenAnyObservable(x => x.ViewContractObservable)
            .Do(x => _viewContract = x)
            .StartWith(ViewContract);
        var viewModelAndContract = currentViewModelChanged.CombineLatest(
            viewContractChanged,
            static (viewModel, contract) => (viewModel, contract));

        if (ModeDetector.InUnitTestRunner())
        {
            _ = viewModelAndContract
                .DistinctUntilChanged()
                .Subscribe(ResolveViewForViewModel, RxState.DefaultExceptionHandler.OnNext);
            return;
        }

        _ = this.WhenActivated((d) =>
            d(viewModelAndContract.DistinctUntilChanged().Subscribe(
                ResolveViewForViewModel,
                RxState.DefaultExceptionHandler.OnNext)));
    }

    /// <summary>Gets or sets the <see cref="RoutingState"/> of the view model stack.</summary>
    public RoutingState Router
    {
        get => (RoutingState)GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    /// <summary>Gets or sets the content displayed whenever there is no page currently routed.</summary>
    public object DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    /// <summary>Gets or sets the view contract observable.</summary>
    /// <value>
    /// The view contract observable.
    /// </value>
    public IObservable<string?> ViewContractObservable
    {
        get => (IObservable<string?>)GetValue(ViewContractObservableProperty);
        set => SetValue(ViewContractObservableProperty, value);
    }

    /// <summary>Gets or sets the view contract.</summary>
    public string? ViewContract
    {
        get => _viewContract;
        set => ViewContractObservable = Observable.Return(value);
    }

    /// <summary>Gets or sets the view locator.</summary>
    /// <value>
    /// The view locator.
    /// </value>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>Creates an observable that publishes the current view contract when the host size changes.</summary>
    /// <param name="add">Adds a size-changed handler.</param>
    /// <param name="remove">Removes a size-changed handler.</param>
    /// <param name="getValue">Gets the current view contract.</param>
    /// <returns>The view contract observable.</returns>
    private static IObservable<string?> CreateViewContractObservable(
        Action<SizeChangedEventHandler> add,
        Action<SizeChangedEventHandler> remove,
        Func<string?> getValue) =>
        ObservableFactory.CreateWithState<string?, (Action<SizeChangedEventHandler> Add, Action<SizeChangedEventHandler> Remove, Func<string?> GetValue)>(
            (add, remove, getValue),
            static (state, observer) =>
            {
                SizeChangedEventHandler handler = (_, _) => observer.OnNext(state.GetValue());
                state.Add(handler);
                return Disposable.Create(
                    (state.Remove, Handler: handler),
                    static subscription => subscription.Remove(subscription.Handler));
            });

    /// <summary>Resolves and displays the view for the supplied routable view model and contract.</summary>
    /// <param name="x">The view model and contract values used for view resolution.</param>
    private void ResolveViewForViewModel((IRoutableViewModel? viewModel, string? contract) x)
    {
        if (x.viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
        var view = viewLocator.ResolveView(x.viewModel, x.contract)
            ?? viewLocator.ResolveView(x.viewModel)
            ?? throw new InvalidOperationException($"Couldn't find view for '{x.viewModel}'.");
        view.ViewModel = x.viewModel;
        Content = view;
    }
}

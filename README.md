[![Build](https://github.com/reactiveui/reactiveui.uno/actions/workflows/ci-build.yml/badge.svg)](https://github.com/reactiveui/reactiveui.uno/actions/workflows/ci-build.yml)
[![Code Coverage](https://codecov.io/gh/reactiveui/reactiveui.uno/branch/main/graph/badge.svg)](https://codecov.io/gh/reactiveui/reactiveui.uno)
[![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://reactiveui.net/contribute)
[![](https://img.shields.io/badge/chat-slack-blue.svg)](https://reactiveui.net/slack)
[![ReactiveUI.Uno](https://img.shields.io/nuget/v/ReactiveUI.Uno.svg)](https://www.nuget.org/packages/ReactiveUI.Uno/)
[![ReactiveUI.Uno.Reactive](https://img.shields.io/nuget/v/ReactiveUI.Uno.Reactive.svg)](https://www.nuget.org/packages/ReactiveUI.Uno.Reactive/)

<br>
<a href="https://github.com/reactiveui/reactiveui">
  <img width="160" height="160" src="https://raw.githubusercontent.com/reactiveui/styleguide/master/logo/main.png">
</a>
<br>

# ReactiveUI for Uno Platform

ReactiveUI.Uno integrates [ReactiveUI](https://reactiveui.net/) with the [Uno Platform](https://platform.uno/) so the same MVVM code can run on Windows, desktop, WebAssembly, Android, and Apple targets. It provides Uno-aware view bases, activation, routing hosts, dependency-property observation, binding hooks, boolean visibility conversion, scheduler integration, resource dictionaries, and suspension storage.

The package family has two variants:

| Package | Use when | Reactive foundation |
| --- | --- | --- |
| `ReactiveUI.Uno` | New Uno applications or libraries that should use ReactiveUI 24 with `ReactiveUI.Primitives` and avoid a direct `System.Reactive` dependency. | `ReactiveUI`, `ReactiveUI.Primitives`, `ReactiveUI.Primitives.Blazor` on browser WebAssembly targets. |
| `ReactiveUI.Uno.Reactive` | Existing ReactiveUI/System.Reactive applications that need Rx-compatible `Unit`, `IScheduler`, and Rx operators while sharing the same Uno integration code. | `ReactiveUI.Reactive`, `ReactiveUI.Primitives.Reactive`, `ReactiveUI.Primitives.Blazor.Reactive` on browser WebAssembly targets. |

Both packages use the same source code. `ReactiveUI.Uno.Reactive` compiles it with the `REACTIVE_SHIM` symbol and publishes the public types under `ReactiveUI.Uno.Reactive`.

## Supported Targets

The packages are built as Uno single-project libraries. The solution currently targets:

| Target family | Target frameworks |
| --- | --- |
| Cross-platform .NET | `net9.0`, `net9.0-desktop`, `net10.0`, `net10.0-desktop`, `net10.0-browserwasm`, `net11.0`, `net11.0-desktop`, `net11.0-browserwasm` |
| Windows desktop | `net9.0-windows10.0.19041.0`, `net10.0-windows10.0.19041.0`, `net11.0-windows10.0.19041.0` |
| Android | `net10.0-android`, `net11.0-android` |
| iOS | `net10.0-ios`, `net11.0-ios` |

Windows targets are added only when building on Windows. Apple mobile targets are added when building on macOS or Windows.

## Install

Install one package, not both, into the Uno project that owns your application startup and views.

```powershell
dotnet add package ReactiveUI.Uno
```

Use the Rx-compatible variant for existing System.Reactive-facing code:

```powershell
dotnet add package ReactiveUI.Uno.Reactive
```

The repository uses central package management. At the time of this branch, the core package references are:

| Package | Version |
| --- | --- |
| `ReactiveUI` | `24.0.0-beta.3` |
| `ReactiveUI.Reactive` | `24.0.0-beta.3` |
| `ReactiveUI.Primitives` | `6.0.0` |
| `ReactiveUI.Primitives.Reactive` | `6.0.0` |
| `ReactiveUI.Primitives.Blazor` | `6.0.0` |
| `ReactiveUI.Primitives.Blazor.Reactive` | `6.0.0` |

## Namespace Map

| Scenario | Main namespaces |
| --- | --- |
| Lean package | `ReactiveUI`, `ReactiveUI.Builder`, `ReactiveUI.Primitives`, `ReactiveUI.Primitives.Concurrency`, `ReactiveUI.Uno` |
| Rx-compatible package | `ReactiveUI.Reactive`, `ReactiveUI.Builder`, `ReactiveUI.Reactive.Builder`, `ReactiveUI.Primitives.Reactive`, `ReactiveUI.Primitives.Reactive.Concurrency`, `ReactiveUI.Uno.Reactive` |
| XAML namespace | `http://reactiveui.net` on non-Windows targets, or `using:ReactiveUI.Uno` / `using:ReactiveUI.Uno.Reactive` |

`ReactiveUI.Uno` uses `ReactiveUI.Primitives.RxVoid` for command completion values and `ReactiveUI.Primitives.Concurrency.ISequencer` for scheduling. `ReactiveUI.Uno.Reactive` uses `System.Reactive.Unit` and `System.Reactive.Concurrency.IScheduler` through the `.Reactive` packages.

## Application Startup

Call `WithUno(Window)` during application startup. It registers Uno services, configures the main-thread scheduler, configures the task-pool scheduler, registers binding converters and hooks, and adds the ReactiveUI Uno resource dictionary after the startup window activates.

```csharp
using ReactiveUI.Builder;
using ReactiveUI.Uno;

public partial class App : Application
{
    private Window? _window;

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        _window ??= new Window();

        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithUno(_window)
            .WithDefaultIScreen()
            .BuildApp();

        _window.Content = new MainView
        {
            ViewModel = new MainViewModel()
        };

        _window.Activate();
    }
}
```

The `.Reactive` startup shape is the same, but the view and library types come from `ReactiveUI.Uno.Reactive`.

```csharp
using ReactiveUI.Builder;
using ReactiveUI.Uno.Reactive;

_ = RxAppBuilder.CreateReactiveUIBuilder()
    .WithUno(window)
    .WithRegistration(mutable => mutable.RegisterConstant<IScreen>(new AppBootstrapper()))
    .BuildApp();
```

## API Reference

### Builder and Registration

| API | Package namespace | Purpose |
| --- | --- | --- |
| `UnoReactiveUIBuilderExtensions.WithUno(Window startupWindow)` | `ReactiveUI.Builder` | Registers Uno services, the platform scheduler, task-pool scheduler, resource dictionary, binding converters, binding hooks, activation, dependency-property observation, and suspension storage. |
| `UnoReactiveUIBuilderExtensions.WithUnoScheduler()` | `ReactiveUI.Builder` | Registers only the current Uno main-thread scheduler. Use this for advanced hosts that manage services and resources separately. |
| `UnoReactiveUIBuilderExtensions.WithDefaultIScreen()` | `ReactiveUI.Builder` | Registers a default `IScreen` backed by `AppBootstrapper`. |
| `UnoReactiveUIBuilderExtensions.UnoMainThreadScheduler` | `ReactiveUI.Builder` | Non-Windows scheduler facade that resolves `UnoDispatcherScheduler.Current` lazily. |
| `UnoReactiveUIBuilderExtensions.UnoWinUIMainThreadScheduler` | `ReactiveUI.Builder` | Windows scheduler facade that resolves `UnoWinUIDispatcherScheduler.Current` lazily. |
| `Registrations` | `ReactiveUI.Uno` or `ReactiveUI.Uno.Reactive` | Implements `IWantsToRegisterStuff`; registers platform services, converters, hooks, and `WinRTAppDataDriver`. Usually used through `WithUno`. |
| `AppBootstrapper` | `ReactiveUI.Uno` or `ReactiveUI.Uno.Reactive` | `ReactiveObject` and `IScreen` implementation with a `RoutingState Router`. |

### Views and Host Controls

| API | Members | Purpose |
| --- | --- | --- |
| `ReactivePage<TViewModel>` | `ViewModelProperty`, `ViewModel`, `BindingRoot`, `IViewFor.ViewModel` | Uno `Page` base class that implements `IViewFor<TViewModel>` and wires an empty activation block so `WhenActivated` is available. |
| `ReactiveUserControl<TViewModel>` | `ViewModelProperty`, `ViewModel`, `BindingRoot`, `IViewFor.ViewModel` | Uno `UserControl` base class that implements `IViewFor<TViewModel>`. |
| `RoutedViewHost` | `Router`, `DefaultContent`, `ViewContract`, `ViewContractObservable`, `ViewLocator` | `TransitioningContentControl` that displays the view for `Router.CurrentViewModel`. |
| `ViewModelViewHost` | `ViewModel`, `DefaultContent`, `ViewContract`, `ViewContractObservable`, `ViewLocator` | `TransitioningContentControl` that displays the view for a single view model object, useful inside data templates. |
| `TransitioningContentControl` | Inherits `ContentControl` | Base content host used by `RoutedViewHost` and `ViewModelViewHost`. |
| `ReactiveUIUnoDictionary` | `Source` is set to the embedded package dictionary | Loads the package resource dictionary. `WithUno(Window)` adds it automatically. |

### Activation, Binding, and Platform Services

| API | Members | Purpose |
| --- | --- | --- |
| `ActivationForViewFetcher` | `GetAffinityForView(Type)`, `GetActivationForView(IActivatableView)` | Activates `FrameworkElement` views when they are loading, visible to hit testing, and not unloaded. |
| `DependencyObjectObservableForProperty` | `GetAffinityForObject(...)`, `GetNotificationForProperty(...)` | Observes Uno dependency-property changes for `WhenAnyValue`, bindings, and property observation. Falls back to POCO observation when needed. |
| `AutoDataTemplateBindingHook` | `DefaultItemTemplate`, `ExecuteHook(...)` | Automatically assigns a `DataTemplate` containing `ViewModelViewHost` when an `ItemsControl.ItemsSource` binding has no template or display member path. |
| `BooleanToVisibilityTypeConverter` | `GetAffinityForObjects()`, `TryConvert(bool, object?, out Visibility)` | Converts `bool` values to `Visibility.Visible` or `Visibility.Collapsed`. |
| `BooleanToVisibilityHint` | `None`, `Inverse` | Optional conversion hint for inverted boolean visibility conversion. |
| `PlatformOperations` | `GetOrientation()` | Supplies the current display orientation for view contract selection where platform APIs are available. |

### Schedulers

| API | Platform | Lean package behavior | `.Reactive` package behavior |
| --- | --- | --- | --- |
| `UnoDispatcherScheduler` | Non-Windows Uno targets using `Windows.UI.Core.CoreDispatcher` | Implements `ISequencer`, exposes `Current`, `Dispatcher`, `Priority`, `Now`, `Timestamp`, `Schedule(IWorkItem)`, `Schedule(IWorkItem, long)`, and `SchedulePeriodic<TState>(...)`. | Implements the Rx scheduler surface through the `IScheduler` alias, exposing `Current`, `Dispatcher`, `Priority`, `Now`, `Schedule<TState>(...)`, due-time overloads, and `SchedulePeriodic<TState>(...)`. |
| `UnoWinUIDispatcherScheduler` | Windows targets using `Microsoft.UI.Dispatching.DispatcherQueue` | Implements `ISequencer`, exposes `Current`, `DispatcherQueue`, `Priority`, `Now`, `Timestamp`, `Schedule(IWorkItem)`, `Schedule(IWorkItem, long)`, and `SchedulePeriodic<TState>(...)`. | Implements the Rx scheduler surface through the `IScheduler` alias, exposing `Current`, `DispatcherQueue`, `Priority`, `Now`, `Schedule<TState>(...)`, due-time overloads, and `SchedulePeriodic<TState>(...)`. |

On browser WebAssembly targets, `WithUno(Window)` uses `ReactiveUI.Primitives.Blazor.Concurrency.BlazorRendererSequencer` in the lean package and `ReactiveUI.Primitives.Blazor.Reactive.Concurrency.BlazorRendererSequencer` in the `.Reactive` package.

### Suspension Storage

| API | Return type in `ReactiveUI.Uno` | Return type in `ReactiveUI.Uno.Reactive` | Purpose |
| --- | --- | --- | --- |
| `WinRTAppDataDriver.LoadState()` | `IObservable<object?>` | `IObservable<object?>` | Loads DataContract XML state from `ApplicationData.Current.RoamingFolder/appData.xmlish`. This path is marked `RequiresDynamicCode` and `RequiresUnreferencedCode`. |
| `WinRTAppDataDriver.SaveState<T>(T state)` | `IObservable<RxVoid>` | `IObservable<Unit>` | Saves DataContract XML state to `appData.xmlish`. This path is marked `RequiresDynamicCode` and `RequiresUnreferencedCode`. |
| `WinRTAppDataDriver.LoadState<T>(JsonTypeInfo<T> typeInfo)` | `IObservable<T?>` | `IObservable<T?>` | Loads JSON state from `appData.json` using source-generated `JsonTypeInfo<T>`. Prefer this for AOT/trimming. |
| `WinRTAppDataDriver.SaveState<T>(T state, JsonTypeInfo<T> typeInfo)` | `IObservable<RxVoid>` | `IObservable<Unit>` | Saves JSON state using source-generated `JsonTypeInfo<T>`. Prefer this for AOT/trimming. |
| `WinRTAppDataDriver.InvalidateState()` | `IObservable<RxVoid>` | `IObservable<Unit>` | Deletes both XML and JSON persisted state files when present. |

## Feature Examples

### Reactive View Models

Use `ReactiveObject`, `RaiseAndSetIfChanged`, `WhenAnyValue`, `ObservableAsPropertyHelper`, and `ReactiveCommand` from ReactiveUI. In the lean package, use `ReactiveUI.Primitives.RxVoid` for command completion values.

```csharp
using ReactiveUI;
using ReactiveUI.Primitives;

public sealed class DeviceViewModel : ReactiveObject, IDisposable
{
    private readonly ObservableAsPropertyHelper<bool> _isCritical;
    private string _name = "Pump A17";
    private double _pressure;

    public DeviceViewModel()
    {
        _isCritical = this.WhenAnyValue(x => x.Pressure)
            .Select(static pressure => pressure > 56)
            .ToProperty(this, static x => x.IsCritical);

        Refresh = ReactiveCommand.CreateFromTask(RefreshAsync);
    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public double Pressure
    {
        get => _pressure;
        set => this.RaiseAndSetIfChanged(ref _pressure, value);
    }

    public bool IsCritical => _isCritical.Value;

    public ReactiveCommand<RxVoid, RxVoid> Refresh { get; }

    public void Dispose()
    {
        _isCritical.Dispose();
        Refresh.Dispose();
    }

    private Task RefreshAsync()
    {
        Pressure = 42.5;
        return Task.CompletedTask;
    }
}
```

The `.Reactive` package uses the same ReactiveUI concepts with `System.Reactive.Unit`.

```csharp
using ReactiveUI.Reactive;
using System.Reactive;

public ReactiveCommand<Unit, Unit> Refresh { get; } =
    ReactiveCommand.Create(() => Unit.Default);
```

### ReactivePage

`ReactivePage<TViewModel>` is the recommended base for page-level views. For Uno XAML, create a non-generic base class and use that base in XAML.

```csharp
using ReactiveUI;
using ReactiveUI.Uno;

public sealed class MainPageBase : ReactivePage<MainViewModel>
{
}

public sealed partial class MainPage : MainPageBase
{
    public MainPage()
    {
        InitializeComponent();
        ViewModel = new MainViewModel();

        this.WhenActivated(disposables =>
        {
            disposables(this.OneWayBind(ViewModel, vm => vm.Title, view => view.TitleText.Text));
            disposables(this.BindCommand(ViewModel, vm => vm.Refresh, view => view.RefreshButton));
        });
    }
}
```

```xml
<views:MainPageBase
    x:Class="MyApp.Views.MainPage"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:views="using:MyApp.Views">
    <StackPanel>
        <TextBlock x:Name="TitleText" />
        <Button x:Name="RefreshButton" Content="Refresh" />
    </StackPanel>
</views:MainPageBase>
```

### ReactiveUserControl

`ReactiveUserControl<TViewModel>` gives reusable controls the same `ViewModel`, `BindingRoot`, and activation pattern.

```csharp
using ReactiveUI;
using ReactiveUI.Uno;

public sealed class DeviceCardBase : ReactiveUserControl<DeviceViewModel>
{
}

public sealed partial class DeviceCard : DeviceCardBase
{
    public DeviceCard()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            disposables(this.OneWayBind(ViewModel, vm => vm.Name, view => view.NameText.Text));
            disposables(this.OneWayBind(ViewModel, vm => vm.Pressure, view => view.ValueText.Text));
        });
    }
}
```

### RoutedViewHost Navigation

`RoutedViewHost` displays the view for `RoutingState.CurrentViewModel`. Register views for routable view models, then bind the host to `IScreen.Router`.

```csharp
using ReactiveUI;
using ReactiveUI.Builder;
using ReactiveUI.Primitives;
using ReactiveUI.Uno;
using Splat;

public sealed class ShellViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new();

    public ReactiveCommand<RxVoid, IRoutableViewModel> OpenDashboard { get; }

    public ShellViewModel()
    {
        OpenDashboard = ReactiveCommand.CreateFromObservable(
            () => Router.Navigate.Execute(new DashboardViewModel(this)));
    }
}

_ = RxAppBuilder.CreateReactiveUIBuilder()
    .WithUno(window)
    .WithRegistration(mutable =>
    {
        mutable.Register(() => new DashboardView(), typeof(IViewFor<DashboardViewModel>));
    })
    .BuildApp();
```

```csharp
using ReactiveUI.Uno;

var host = new RoutedViewHost
{
    Router = shellViewModel.Router,
    DefaultContent = new TextBlock { Text = "Choose a route." }
};
```

Use `ViewContract` or `ViewContractObservable` when the same view model has different views for different display modes.

```csharp
var host = new RoutedViewHost
{
    Router = shell.Router,
    ViewContract = "Compact"
};
```

### ViewModelViewHost

`ViewModelViewHost` resolves and displays a view for a single object. It is useful in item templates and dashboards where tiles own their own view models.

```csharp
using ReactiveUI.Uno;

var host = new ViewModelViewHost
{
    ViewModel = selectedDevice,
    DefaultContent = new TextBlock { Text = "No device selected." }
};
```

You can override the view lookup with a specific locator.

```csharp
host.ViewLocator = ReactiveUI.ViewLocator.Current;
host.ViewContract = "Detailed";
```

### Automatic ItemsControl Templates

`AutoDataTemplateBindingHook` is registered by `WithUno(Window)`. When an `ItemsControl` binds `ItemsSource` and does not already have `DisplayMemberPath`, `ItemTemplate`, or `ItemTemplateSelector`, the hook assigns a template that hosts each item in `ViewModelViewHost`.

```csharp
this.WhenActivated(disposables =>
{
    disposables(this.OneWayBind(ViewModel, vm => vm.Devices, view => view.DeviceList.ItemsSource));
});
```

```csharp
var list = new ListView();

// No ItemTemplate is required when each item has an IViewFor<T> registration.
list.ItemsSource = viewModel.Devices;
```

Register the view for the item view model.

```csharp
mutable.Register(() => new DeviceCard(), typeof(IViewFor<DeviceViewModel>));
```

### Activation

`ActivationForViewFetcher` integrates Uno `FrameworkElement` loading, unloading, and hit-test visibility with ReactiveUI activation. Use `WhenActivated` in views and put subscriptions, bindings, and interaction handlers inside the activation block.

```csharp
this.WhenActivated(disposables =>
{
    disposables(ViewModel!.ConfirmAcknowledge.RegisterHandler(async interaction =>
    {
        var approved = await ShowAcknowledgeDialogAsync(interaction.Input);
        interaction.SetOutput(approved);
    }));

    disposables(ViewModel.Refresh.Execute().Subscribe());
});
```

### Dependency-Property Observation

`DependencyObjectObservableForProperty` lets ReactiveUI observe dependency properties when they have a backing `DependencyProperty` field or property named `<PropertyName>Property`.

```csharp
this.WhenAnyValue(view => view.IsHitTestVisible)
    .DistinctUntilChanged()
    .Subscribe(isVisible => Debug.WriteLine($"Visible to hit testing: {isVisible}"));
```

This is used internally by activation and normal ReactiveUI bindings. If a property is not a dependency property, the integration falls back to POCO observation.

### Boolean to Visibility

`BooleanToVisibilityTypeConverter` is registered automatically. It maps `true` to `Visibility.Visible` and `false` to `Visibility.Collapsed`. Pass `BooleanToVisibilityHint.Inverse` to invert the result.

```csharp
using ReactiveUI.Uno;

var converter = new BooleanToVisibilityTypeConverter();

if (converter.TryConvert(viewModel.HasNoResults, BooleanToVisibilityHint.Inverse, out var visibility))
{
    ResultsPanel.Visibility = visibility;
}
```

### Platform Orientation and View Contracts

`PlatformOperations.GetOrientation()` returns the current display orientation where the platform can supply it. `RoutedViewHost` and `ViewModelViewHost` use this through `ViewContractObservable` to re-resolve views when the orientation changes.

```csharp
var platform = Locator.Current.GetService<IPlatformOperations>();
var orientation = platform?.GetOrientation();

var host = new ViewModelViewHost
{
    ViewModel = viewModel,
    ViewContract = orientation
};
```

### Resource Dictionary

`WithUno(Window)` adds `ReactiveUIUnoDictionary` after the startup window activates. Add it manually only when you intentionally bypass `WithUno(Window)`.

```csharp
Application.Current.Resources.MergedDictionaries.Add(new ReactiveUIUnoDictionary());
```

The dictionary references `ms-appx:///ReactiveUI.Uno/Resources/ReactiveUI.Uno.xaml` in the lean package and `ms-appx:///ReactiveUI.Uno.Reactive/Resources/ReactiveUI.Uno.Reactive.xaml` in the `.Reactive` package.

### Main-Thread Scheduling

Use the scheduler registered by `WithUno(Window)` through ReactiveUI scheduler APIs. In the lean package, the scheduler is an `ISequencer`; in the `.Reactive` package, it is Rx-compatible.

```csharp
using ReactiveUI.Primitives.Concurrency;
using ReactiveUI.Uno;

ISequencer ui = UnoReactiveUIBuilderExtensions.UnoMainThreadScheduler;

var subscription = this.WhenAnyValue(view => view.ViewModel!.StatusMessage)
    .ObserveOn(ui)
    .Subscribe(message => StatusText.Text = message);
```

On Windows, the concrete scheduler uses `DispatcherQueue`.

```csharp
#if WINDOWS
var scheduler = UnoWinUIDispatcherScheduler.Current;
_ = scheduler.SchedulePeriodic(0, TimeSpan.FromSeconds(1), count =>
{
    StatusText.Text = $"Tick {count + 1}";
    return count + 1;
});
#endif
```

On non-Windows Uno targets, the concrete scheduler uses `CoreDispatcher`.

```csharp
#if !WINDOWS
var scheduler = UnoDispatcherScheduler.Current;
_ = scheduler.SchedulePeriodic(0, TimeSpan.FromSeconds(1), count =>
{
    StatusText.Text = $"Tick {count + 1}";
    return count + 1;
});
#endif
```

The `.Reactive` package supports the System.Reactive scheduler shape.

```csharp
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using ReactiveUI.Uno.Reactive;

IScheduler scheduler =
#if WINDOWS
    UnoWinUIDispatcherScheduler.Current;
#else
    UnoDispatcherScheduler.Current;
#endif

var subscription = Observable.Interval(TimeSpan.FromSeconds(1), scheduler)
    .Subscribe(_ => ClockText.Text = DateTimeOffset.Now.ToString("T", CultureInfo.CurrentCulture));
```

### Suspension State

`WinRTAppDataDriver` is registered as `ISuspensionDriver` by `WithUno(Window)`. Prefer the JSON `JsonTypeInfo<T>` overloads for AOT and trimming because they do not carry the DataContract serializer attributes.

```csharp
using System.Text.Json.Serialization;
using ReactiveUI.Primitives;
using ReactiveUI.Uno;

public sealed record AppState(string CurrentDevice, bool IsStreaming);

[JsonSerializable(typeof(AppState))]
public sealed partial class AppStateJsonContext : JsonSerializerContext;

public sealed class StateService
{
    private readonly WinRTAppDataDriver _driver = new();

    public IObservable<RxVoid> Save(AppState state) =>
        _driver.SaveState(state, AppStateJsonContext.Default.AppState);

    public IObservable<AppState?> Load() =>
        _driver.LoadState(AppStateJsonContext.Default.AppState);

    public IObservable<RxVoid> Clear() =>
        _driver.InvalidateState();
}
```

The `.Reactive` variant returns `System.Reactive.Unit` for save and invalidate operations.

```csharp
using System.Reactive;
using ReactiveUI.Uno.Reactive;

IObservable<Unit> saved = new WinRTAppDataDriver()
    .SaveState(state, AppStateJsonContext.Default.AppState);
```

The legacy XML overloads are available for compatibility:

```csharp
IObservable<object?> loaded = new WinRTAppDataDriver().LoadState();
```

Use them only when DataContract serialization is acceptable for your platform and deployment mode.

## Choosing Lean or Reactive

Choose `ReactiveUI.Uno` when:

- You are building a new app on ReactiveUI 24.
- You want `ReactiveUI.Primitives` scheduler and disposable types.
- You do not want to expose `System.Reactive.Unit` or `System.Reactive.Concurrency.IScheduler` in your app APIs.
- You target browser WebAssembly and want the Primitives Blazor scheduler without adding `System.Reactive.Wasm`.

Choose `ReactiveUI.Uno.Reactive` when:

- You already have Rx pipelines based on `System.Reactive.Linq.Observable`.
- Your public APIs already expose `System.Reactive.Unit` or `IScheduler`.
- You want a migration path with the same Uno controls and services while keeping existing Rx call sites.
- You are demonstrating or consuming `ReactiveUI.Reactive` features.

The source-compatible migration pattern is:

```csharp
#if REACTIVE_SHIM
namespace MyApp.Uno.Reactive;
#else
namespace MyApp.Uno;
#endif
```

Bind neutral names in project files or local aliases when sharing code:

```csharp
#if REACTIVE_SHIM
using RxUnit = System.Reactive.Unit;
#else
using RxUnit = ReactiveUI.Primitives.RxVoid;
#endif
```

## Example Applications

Examples live under `src/Examples` and are not included in package coverage.

| Example | Package | What it demonstrates |
| --- | --- | --- |
| `ReactiveUI.Uno.SQLiteStudio` | `ReactiveUI.Uno` | Lean Primitives usage, `WithUno(Window)`, service registration, `ReactiveCommand`, `RaiseAndSetIfChanged`, code-built Uno views, `WhenActivated`, `Bind`, `BindCommand`, `OneWayBind`, SQLite operations, CSV export, and unpacked desktop/window startup patterns. |
| `ReactiveUI.Uno.Reactive.IoTDashboard` | `ReactiveUI.Uno.Reactive` | Rx-compatible package usage, live local telemetry, `Observable.Interval`, `Publish().RefCount()`, `ReactiveObject`, `ReactiveCommand`, `ObservableAsPropertyHelper`, `Interaction`, `WhenActivated`, command binding, one-way and two-way binding, main-thread observation, and disposal of subscriptions and commands. |

The IoT dashboard shows the `.Reactive` package in a live-data scenario:

```csharp
_telemetrySubscription = _telemetry.Readings
    .Where(_ => IsStreaming)
    .ObserveOn(ReactiveUI.Reactive.RxSchedulers.MainThreadScheduler)
    .Subscribe(ApplyReading, exception => StatusMessage = exception.Message);
```

It also demonstrates an interaction-backed command:

```csharp
AcknowledgeAlert = ReactiveCommand.CreateFromObservable(
    AcknowledgeAlertObservable,
    this.WhenAnyValue(x => x.HasActiveAlert));

private IObservable<Unit> AcknowledgeAlertObservable()
{
    var alert = Alerts.FirstOrDefault(static item => !item.IsAcknowledged);
    return alert is null
        ? Observable.Return(Unit.Default)
        : ConfirmAcknowledge.Handle(alert.Event)
            .Select(approved => CompleteAcknowledge(alert, approved));
}
```

## Build and Test

This repository uses `src/ReactiveUI.Uno.slnx` and Microsoft Testing Platform with TUnit. Run commands from `src` unless the command includes `src/` paths.

```powershell
cd src
dotnet workload restore
dotnet restore ReactiveUI.Uno.slnx
```

On Windows, use Visual Studio MSBuild for the full Release solution build:

```powershell
& 'C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\amd64\MSBuild.exe' .\ReactiveUI.Uno.slnx /restore /p:Configuration=Release /warnaserror /m /v:minimal
```

Run the focused TUnit/MTP coverage checks for the two package projects:

```powershell
dotnet test --project .\tests\ReactiveUI.Uno.Tests\ReactiveUI.Uno.Tests.csproj -c Release --coverage --coverage-output-format cobertura -- --report-trx
dotnet test --project .\tests\ReactiveUI.Uno.Reactive.Tests\ReactiveUI.Uno.Reactive.Tests.csproj -c Release --coverage --coverage-output-format cobertura -- --report-trx
```

The coverage configuration includes `ReactiveUI.Uno` and `ReactiveUI.Uno.Reactive`; examples are intentionally excluded.

## Thanks

We want to thank the following contributors and libraries that help make ReactiveUI.Uno possible:

- **Uno Platform**: [Uno Platform](https://platform.uno/) - The underlying cross-platform UI framework.
- **ReactiveUI**: [ReactiveUI](https://github.com/reactiveui/reactiveui) - The MVVM framework.
- **ReactiveUI.Primitives**: [ReactiveUI.Primitives](https://www.nuget.org/packages/ReactiveUI.Primitives/) - The lean observable, disposable, scheduler, and signal foundation used by `ReactiveUI.Uno`.
- **ReactiveUI.Reactive**: [ReactiveUI.Reactive](https://www.nuget.org/packages/ReactiveUI.Reactive/) - The System.Reactive-compatible ReactiveUI package used by `ReactiveUI.Uno.Reactive`.
- **Splat**: [Splat](https://github.com/reactiveui/splat) - Cross-platform service location and utilities.

## Sponsorship

The core team members, ReactiveUI contributors, and ecosystem contributors do this open-source work in their free time. If you use ReactiveUI and would like us to invest more time in it, please donate.

[Become a sponsor](https://github.com/sponsors/reactivemarbles).

This is how we use the donations:

- Allow the core team to work on ReactiveUI.
- Thank contributors if they invested a large amount of time in contributing.
- Support projects in the ecosystem.


## Core Team

<table>
  <tbody>
    <tr>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/glennawatson.png?s=150">
        <br>
        <a href="https://github.com/glennawatson">Glenn Watson</a>
        <p>Melbourne, Australia</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/chrispulman.png?s=150">
        <br>
        <a href="https://github.com/chrispulman">Chris Pulman</a>
        <p>United Kingdom</p>
      </td>
    </tr>
    <tr>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/rlittlesii.png?s=150">
        <br>
        <a href="https://github.com/rlittlesii">Rodney Littles II</a>
        <p>Texas, USA</p>
      </td>
      <td align="center" valign="top" width="105">
        <img width="100" height="100" src="https://github.com/cabauman.png?s=150">
        <br>
        <a href="https://github.com/cabauman">Colt Bauman</a>
        <p>South Korea</p>
      </td>
    </tr>
  </tbody>
</table>

## Support

If you have a question, please see whether [GitHub Discussions](https://github.com/reactiveui/ReactiveUI.Uno/discussions) or [GitHub issues](https://github.com/reactiveui/ReactiveUI.Uno/issues) have already answered it.

If you want to discuss something or need help, use the [ReactiveUI Slack room](https://reactiveui.net/slack).

Please do not open GitHub issues for support requests.

## Contribute

ReactiveUI.Uno is developed under an OSI-approved open source license, making it freely usable and distributable, including commercial use.

Before submitting a pull request, open a [GitHub issue](https://github.com/reactiveui/ReactiveUI/issues/new/choose) to discuss the change. We are first-time PR contributor friendly.

See the [Contribution Guidelines](https://www.reactiveui.net/contribute/) for more information.

## License

ReactiveUI.Uno is licensed under the [MIT License](https://github.com/reactiveui/ReactiveUI.Uno/blob/main/LICENSE).

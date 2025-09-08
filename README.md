[![Build](https://github.com/reactiveui/reactiveui.uno/actions/workflows/ci-build.yml/badge.svg)](https://github.com/reactiveui/reactiveui.uno/actions/workflows/ci-build.yml)
[![Code Coverage](https://codecov.io/gh/reactiveui/reactiveui.uno/branch/main/graph/badge.svg)](https://codecov.io/gh/reactiveui/reactiveui.uno)
[![#yourfirstpr](https://img.shields.io/badge/first--timers--only-friendly-blue.svg)](https://reactiveui.net/contribute)
[![](https://img.shields.io/badge/chat-slack-blue.svg)](https://reactiveui.net/slack)
[![NuGet](https://img.shields.io/nuget/v/ReactiveUI.Uno.svg)](https://www.nuget.org/packages/ReactiveUI.Uno/)

<br>
<a href="https://github.com/reactiveui/reactiveui">
  <img width="160" heigth="160" src="https://raw.githubusercontent.com/reactiveui/styleguide/master/logo/main.png">
</a>
<br>

# ReactiveUI for Uno Platform

This package provides [ReactiveUI](https://reactiveui.net/) bindings for the [Uno Platform](https://platform.uno/), enabling you to build composable, cross-platform model-view-viewmodel (MVVM) applications that run on iOS, Android, WebAssembly, macOS, and Windows.

---
## NuGet Packages

To get started, install the following package into your shared Uno Platform project.

| Platform          | NuGet                  |
| ----------------- | ---------------------- |
| Platform Uno      | [![NuGet](https://img.shields.io/nuget/v/ReactiveUI.Uno.svg)](https://www.nuget.org/packages/ReactiveUI.Uno/)     |

-----

## Tutorial: Mastering ReactiveUI with the Uno Platform

Welcome to the `ReactiveUI.Uno` guide! This tutorial will walk you through setting up a cross-platform application using the Uno Platform with the power of ReactiveUI. We'll start from the basics and build up to a fully reactive application.

`ReactiveUI.Uno` provides the necessary bindings and helpers to seamlessly integrate the ReactiveUI MVVM framework with your Uno Platform projects, enabling you to write elegant, testable, and maintainable code.

### Chapter 1: Getting Started - Your First Reactive View

Let's begin by setting up your project and creating your first reactive view and view model.

#### 1. Installation

First, ensure you have the Uno Platform templates installed and create a new application. Then, add the `ReactiveUI.Uno` package to your project's shared `csproj` file.

```xml
<PackageReference Include="ReactiveUI.Uno" Version="21.0.1" />
```

#### 2. Initialization

Next, initialize ReactiveUI in your `App.cs` startup code. This wires up the necessary services for the Uno Platform.

```csharp
using ReactiveUI;
using Splat;

public App()
{
    // ... existing initialization ...

    var builder = Splat.AppLocator.CurrentMutable.CreateReactiveUIBuilder();
    builder
        .WithUno() // This extension method initializes ReactiveUI for Uno
        .Build();

    // ... more initialization ...
}
```

#### 3. Create a ViewModel

Create a simple view model. Notice how it inherits from `ReactiveObject` and uses `RaiseAndSetIfChanged` to notify the UI of property changes.

```csharp
// MyViewModel.cs
using ReactiveUI;

public class MyViewModel : ReactiveObject
{
    private string _greeting;

    public string Greeting
    {
        get => _greeting;
        set => this.RaiseAndSetIfChanged(ref _greeting, value);
    }

    public MyViewModel()
    {
        Greeting = "Hello, Reactive World!";
    }
}
```

#### 4. Create a Reactive View

Now, let's create a view that binds to this view model. `ReactivePage<TViewModel>` is a base class that makes this easy.

```xml
<rxui:ReactivePage
    x:Class="MyUnoApp.MainPage"
    x:TypeArguments="local:MyViewModel"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:local="using:MyUnoApp"
    xmlns:rxui="using:ReactiveUI.Uno"
    mc:Ignorable="d">

    <StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
        <TextBlock x:Name="GreetingTextBlock" FontSize="24" />
    </StackPanel>
</rxui:ReactivePage>
```

In the code-behind, use `WhenActivated` to set up your bindings. This is the core of a reactive view.

```csharp
// MainPage.xaml.cs
using ReactiveUI;
using System.Reactive.Disposables;

public sealed partial class MainPage : ReactivePage<MyViewModel>
{
    public MainPage()
    {
        this.InitializeComponent();
        ViewModel = new MyViewModel();

        this.WhenActivated(disposables =>
        {
            // Bind the Greeting property of the ViewModel to the Text of the TextBlock
            this.OneWayBind(ViewModel,
                viewModel => viewModel.Greeting,
                view => view.GreetingTextBlock.Text)
                .DisposeWith(disposables);
        });
    }
}
```

Congratulations! You've just created your first reactive UI with `ReactiveUI.Uno`. When you run the app, you'll see the greeting message displayed.

### Chapter 2: Handling User Interaction with ReactiveCommands

Static text is great, but apps need to respond to users. `ReactiveCommand` is the standard way to handle user actions like button clicks in a testable and composable way.

#### 1. Add a Command to the ViewModel

Let's add a command to our view model that generates a new greeting.

```csharp
// MyViewModel.cs
using ReactiveUI;
using System;
using System.Reactive;

public class MyViewModel : ReactiveObject
{
    // ... Greeting property from before ...

    public ReactiveCommand<Unit, Unit> GenerateGreetingCommand { get; }

    public MyViewModel()
    {
        Greeting = "Hello, Reactive World!";

        GenerateGreetingCommand = ReactiveCommand.Create(() =>
        {
            Greeting = $"Hello from Uno! The time is {DateTime.Now.ToLongTimeString()}";
        });
    }
}
```

#### 2. Bind the Command in the View

Now, add a button to your XAML and bind its `Command` property to the new command.

```xml
<StackPanel HorizontalAlignment="Center" VerticalAlignment="Center">
    <TextBlock x:Name="GreetingTextBlock" FontSize="24" />
    <Button x:Name="GenerateGreetingButton" Content="Generate" Margin="0,20,0,0" />
</StackPanel>
```

Update your `WhenActivated` block to bind the button to the command.

```csharp
// MainPage.xaml.cs
this.WhenActivated(disposables =>
{
    // ... existing binding ...

    // Bind the GenerateGreetingButton to the GenerateGreetingCommand
    this.BindCommand(ViewModel,
        viewModel => viewModel.GenerateGreetingCommand,
        view => view.GenerateGreetingButton)
        .DisposeWith(disposables);
});
```

Now, when you click the button, the command will execute, the `Greeting` property will change, and the UI will automatically update.

### Chapter 3: Navigating with `RoutedViewHost`

For more complex applications, you'll need navigation. `RoutedViewHost` is a control that displays a view based on the current state of a `RoutingState` object.

#### 1. Set up a Router

In your main view model (or a dedicated routing service), create a `RoutingState`.

```csharp
// AppViewModel.cs
public class AppViewModel : ReactiveObject, IScreen
{
    public RoutingState Router { get; } = new RoutingState();

    public AppViewModel()
    {
        // Navigate to the initial view model
        Router.Navigate.Execute(new MyViewModel());
    }
}
```

#### 2. Use `RoutedViewHost` in your Main Window

In your main window's XAML, replace the content with a `RoutedViewHost` and bind its `Router` property.

```xml
<Window ...>
    <Grid>
        <rxui:RoutedViewHost Router="{Binding Router}" />
    </Grid>
</Window>
```

You'll also need a way to tell `RoutedViewHost` which view to create for a given view model. This is done by registering views for your view models in your App.cs.

```csharp
// App.cs
Locator.CurrentMutable.Register(() => new MainPage(), typeof(IViewFor<MyViewModel>));
```

Now, as you call `Router.Navigate.Execute(...)`, the `RoutedViewHost` will automatically switch to the correct view.

This tutorial has covered the basics of getting started with `ReactiveUI.Uno`. You've learned how to set up your project, create reactive view models and views, handle user interaction with commands, and manage navigation. From here, you can explore more advanced ReactiveUI features like `WhenAnyValue`, `ObservableAsPropertyHelper`, and more complex command scenarios.

-----

## Thanks

We want to thank the following contributors and libraries that help make ReactiveUI.Uno possible:

### Core Libraries

  - **Uno Platform**: [Uno Platform](https://platform.uno/) - The underlying cross-platform UI framework.
  - **System.Reactive**: [Reactive Extensions for .NET](https://github.com/dotnet/reactive) - The foundation of ReactiveUI's asynchronous API.
  - **Splat**: [Splat](https://github.com/reactiveui/splat) - Cross-platform utilities and service location.
  - **ReactiveUI**: [ReactiveUI](https://github.com/reactiveui/reactiveui) - The core MVVM framework.

-----

## Sponsorship

The core team members, ReactiveUI contributors and contributors in the ecosystem do this open-source work in their free time. If you use ReactiveUI, a serious task, and you'd like us to invest more time on it, please donate. This project increases your income/productivity too. It makes development and applications faster and it reduces the required bandwidth.

[Become a sponsor](https://github.com/sponsors/reactivemarbles).

This is how we use the donations:

  * Allow the core team to work on ReactiveUI
  * Thank contributors if they invested a large amount of time in contributing
  * Support projects in the ecosystem

-----

## Support

If you have a question, please see if any discussions in our [GitHub Discussions](https://github.com/reactiveui/ReactiveUI.Uno/discussions) or [GitHub issues](https://github.com/reactiveui/ReactiveUI.Uno/issues) have already answered it.

If you want to discuss something or just need help, here is our [Slack room](https://reactiveui.net/slack), where there are always individuals looking to help out\!

Please do not open GitHub issues for support requests.

-----

## Contribute

ReactiveUI.Uno is developed under an OSI-approved open source license, making it freely usable and distributable, even for commercial use.

If you want to submit pull requests please first open a [GitHub issue](https://github.com/reactiveui/ReactiveUI/issues/new/choose) to discuss. We are first time PR contributors friendly.

See [Contribution Guidelines](https://www.reactiveui.net/contribute/) for further information how to contribute changes.

-----

## License

ReactiveUI.Uno is licensed under the [MIT License]([https://www.google.com/search?q=LICENSE](https://github.com/reactiveui/ReactiveUI.Uno/blob/main/LICENSE)).

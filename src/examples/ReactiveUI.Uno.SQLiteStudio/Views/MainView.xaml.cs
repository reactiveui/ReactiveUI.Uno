using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables.Fluent;
using ReactiveUI.Uno.SQLiteStudio.Presentation;

namespace ReactiveUI.Uno.SQLiteStudio.Views;

/// <summary>
/// Serves as a base class for main view user controls that are bound to a MainViewModel instance using reactive
/// patterns.
/// </summary>
/// <remarks>This class provides common functionality for main views in applications that use the ReactiveUI
/// framework. It enables data binding and reactive UI updates between the view and the MainViewModel. The class may use
/// reflection internally, which can affect compatibility with ahead-of-time (AOT) compilation environments.</remarks>
[RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
public partial class MainViewBase : ReactiveUserControl<MainViewModel> { }

/// <summary>
/// Represents the main user interface view for the application, providing controls for query editing, execution, and
/// data management.
/// </summary>
/// <remarks>MainView binds UI elements to the associated view model, enabling users to compose and execute
/// queries, export results, and manage database tables. This class uses reflection and may not be compatible with
/// ahead-of-time (AOT) compilation environments.</remarks>
[RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
public sealed partial class MainView : MainViewBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MainView"/> class.
    /// </summary>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public MainView()
    {
        InitializeComponent();

        this.WhenActivated(disposables =>
        {
            // Bindings
            this.Bind(ViewModel, vm => vm.QueryText, v => v.QueryEditor.Text)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.ExecuteQuery, v => v.ExecuteButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.ExportCsv, v => v.ExportButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.ListTables, v => v.ListTablesButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.CreateUsersTable, v => v.CreateUsersButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.DropUsersTable, v => v.DropUsersButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.SampleSelect, v => v.SampleSelectButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.SampleInsert, v => v.SampleInsertButton)
                .DisposeWith(disposables);

            this.BindCommand(ViewModel, vm => vm.SampleDelete, v => v.SampleDeleteButton)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Results, v => v.ResultsList.ItemsSource)
                .DisposeWith(disposables);

            this.OneWayBind(ViewModel, vm => vm.Status, v => v.StatusText.Text)
                .DisposeWith(disposables);
        });
    }
}

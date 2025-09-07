using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Uno.SQLiteStudio.Presentation;

namespace ReactiveUI.Uno.SQLiteStudio.Views;

[RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
public partial class MainViewBase : ReactiveUserControl<MainViewModel> { }

[RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
public sealed partial class MainView : MainViewBase
{
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public MainView()
    {
        this.InitializeComponent();

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

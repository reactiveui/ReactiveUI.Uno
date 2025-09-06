namespace ReactiveUI.Uno.SQLiteStudio.Services;

public interface ICsvExportService
{
    Task ExportAsync(System.Collections.IEnumerable items, string fileName = "results.csv");
}

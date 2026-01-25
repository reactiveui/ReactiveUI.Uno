namespace ReactiveUI.Uno.SQLiteStudio.Services;

/// <summary>
/// Defines a service for exporting a collection of items to a CSV file asynchronously.
/// </summary>
/// <remarks>Implementations of this interface should ensure that the exported CSV file accurately represents each
/// item in the provided collection as a row. The format and mapping of item properties to CSV columns may vary
/// depending on the implementation. This interface is intended for use in scenarios where data needs to be exported in
/// a standard, comma-separated values format for interoperability or reporting purposes.</remarks>
public interface ICsvExportService
{
    /// <summary>
    /// Asynchronously exports the specified collection of items to a CSV file.
    /// </summary>
    /// <param name="items">The collection of items to export. Each item should represent a row in the resulting CSV file. Cannot be null.</param>
    /// <param name="fileName">The name of the CSV file to create. If not specified, defaults to "results.csv". Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous export operation.</returns>
    Task ExportAsync(System.Collections.IEnumerable items, string fileName = "results.csv");
}

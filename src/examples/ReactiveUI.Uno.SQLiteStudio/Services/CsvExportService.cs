using System.Collections;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace ReactiveUI.Uno.SQLiteStudio.Services;

/// <summary>
/// Provides functionality to export collections of data to a CSV file using a singleton service instance.
/// </summary>
/// <remarks>This service is intended for exporting data to CSV format in local application storage. The singleton
/// instance can be accessed via the <see cref="Instance"/> property. The service supports exporting collections of
/// items, including dictionaries with string keys and object values, to a CSV file. Thread safety is ensured for
/// instance access, but concurrent export operations should be managed by the caller if writing to the same
/// file.</remarks>
public sealed class CsvExportService : ICsvExportService
{
    private static readonly Lazy<ICsvExportService> LazyInstance = new(() => new CsvExportService());

    private CsvExportService()
    {
    }

    /// <summary>
    /// Gets the singleton instance of the CSV export service.
    /// </summary>
    /// <remarks>Use this property to access the shared implementation of the ICsvExportService interface. The
    /// instance is lazily initialized and intended for reuse throughout the application.</remarks>
    public static ICsvExportService Instance => LazyInstance.Value;

    /// <summary>
    /// Asynchronously exports the specified collection of items to a CSV file in the local application data folder.
    /// </summary>
    /// <remarks>If an item in the collection is an IReadOnlyDictionary{string, object?}, each key-value pair
    /// is written as a pair of fields in the CSV. Otherwise, the item's string representation is written as a single
    /// field. The CSV file does not include a header row. The file is saved to the user's local application data
    /// folder.</remarks>
    /// <param name="items">The collection of items to export. Each item can be an object or an IReadOnlyDictionary{string, object?}. Cannot
    /// be null.</param>
    /// <param name="fileName">The name of the CSV file to create. Defaults to "results.csv" if not specified.</param>
    /// <returns>A task that represents the asynchronous export operation.</returns>
    public async Task ExportAsync(IEnumerable items, string fileName = "results.csv")
    {
        ArgumentNullException.ThrowIfNull(items);

        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var path = Path.Combine(folder, fileName);

        await using var writer = new StreamWriter(path);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };
        await using var csv = new CsvWriter(writer, config);

        foreach (var item in items)
        {
            if (item is IReadOnlyDictionary<string, object?> dict)
            {
                foreach (var kvp in dict)
                {
                    csv.WriteField(kvp.Key);
                    csv.WriteField(kvp.Value?.ToString() ?? string.Empty);
                }
                csv.NextRecord();
            }
            else
            {
                csv.WriteField(item?.ToString() ?? string.Empty);
                csv.NextRecord();
            }
        }
        await writer.FlushAsync().ConfigureAwait(false);
    }
}

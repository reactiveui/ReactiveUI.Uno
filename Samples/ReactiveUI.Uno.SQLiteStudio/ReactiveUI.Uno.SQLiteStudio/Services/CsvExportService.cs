using System.Collections;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace ReactiveUI.Uno.SQLiteStudio.Services;

public sealed class CsvExportService : ICsvExportService
{
    private static readonly Lazy<ICsvExportService> LazyInstance = new(() => new CsvExportService());

    private CsvExportService()
    {
    }

    public static ICsvExportService Instance => LazyInstance.Value;

    public async Task ExportAsync(IEnumerable items, string fileName = "results.csv")
    {
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var path = Path.Combine(folder, fileName);

        using var writer = new StreamWriter(path);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };
        using var csv = new CsvWriter(writer, config);

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

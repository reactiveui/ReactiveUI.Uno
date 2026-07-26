// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace ReactiveUI.Uno.SQLiteStudio.Services;

/// <summary>Provides functionality to export collections of data to a CSV file.</summary>
/// <remarks>This service is intended for exporting data to CSV format in local application storage. The singleton
/// instance can be accessed via the <see cref="Instance"/> property. The service supports exporting collections of
/// items, including dictionaries with string keys and object values, to a CSV file. Thread safety is ensured for
/// instance access, but concurrent export operations should be managed by the caller if writing to the same
/// file.</remarks>
public sealed class CsvExportService : ICsvExportService
{
    /// <summary>Stores the default CSV export file name.</summary>
    private const string DefaultFileName = "results.csv";

    /// <summary>Stores the lazy singleton CSV export service instance.</summary>
    private static readonly Lazy<ICsvExportService> LazyInstance = new(() => new CsvExportService());

    /// <summary>Initializes a new instance of the <see cref="CsvExportService"/> class.</summary>
    private CsvExportService()
    {
    }

    /// <summary>Gets the singleton instance of the CSV export service.</summary>
    /// <remarks>Use this property to access the shared implementation of the ICsvExportService interface. The
    /// instance is lazily initialized and intended for reuse throughout the application.</remarks>
    public static ICsvExportService Instance => LazyInstance.Value;

    /// <summary>Exports the specified collection of items to the default CSV file.</summary>
    /// <param name="items">The collection of items to export.</param>
    /// <returns>A task that represents the asynchronous export operation.</returns>
    public Task ExportAsync(IEnumerable items) => ExportAsync(items, DefaultFileName);

    /// <summary>Asynchronously exports the specified collection of items to a CSV file.</summary>
    /// <remarks>The CSV file does not include a header row and is saved to local application data.</remarks>
    /// <param name="items">The collection of items to export. Cannot be null.</param>
    /// <param name="fileName">The name of the CSV file to create. Defaults to "results.csv" if not specified.</param>
    /// <returns>A task that represents the asynchronous export operation.</returns>
    public async Task ExportAsync(IEnumerable items, string fileName)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var path = Path.Combine(folder, fileName);

        await using var writer = new StreamWriter(path);
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };
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

                await csv.NextRecordAsync().ConfigureAwait(false);
            }
            else
            {
                csv.WriteField(item?.ToString() ?? string.Empty);
                await csv.NextRecordAsync().ConfigureAwait(false);
            }
        }

        await writer.FlushAsync().ConfigureAwait(false);
    }
}

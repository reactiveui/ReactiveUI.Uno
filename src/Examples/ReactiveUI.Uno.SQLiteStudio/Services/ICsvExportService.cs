// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.
namespace ReactiveUI.Uno.SQLiteStudio.Services;

/// <summary>Defines a service for exporting a collection of items to a CSV file asynchronously.</summary>
/// <remarks>Implementations of this interface should ensure that the exported CSV file accurately represents each
/// item in the provided collection as a row. The format and mapping of item properties to CSV columns may vary
/// depending on the implementation. This interface is intended for use in scenarios where data needs to be exported in
/// a standard, comma-separated values format for interoperability or reporting purposes.</remarks>
public interface ICsvExportService
{
    /// <summary>Asynchronously exports the specified collection of items to a CSV file.</summary>
    /// <param name="items">The collection of items to export. Each item should represent a row in the resulting CSV file. Cannot be null.</param>
    /// <param name="fileName">The name of the CSV file to create. If not specified, defaults to "results.csv". Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous export operation.</returns>
    Task ExportAsync(System.Collections.IEnumerable items, string fileName = "results.csv");
}

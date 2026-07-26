// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

/// <summary>Formats rows for the SQLite Studio result pane.</summary>
internal static class ResultFormatter
{
    /// <summary>Formats result rows for the sample result pane.</summary>
    /// <param name="results">The result rows to format.</param>
    /// <returns>A display string for the result pane.</returns>
    internal static string FormatResults(IEnumerable<object> results)
    {
        var rows = results.Select(static item => item?.ToString() ?? string.Empty);
        var text = string.Join(Environment.NewLine, rows);
        return string.IsNullOrWhiteSpace(text) ? "(no rows)" : text;
    }
}

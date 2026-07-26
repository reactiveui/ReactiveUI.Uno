// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.SQLiteStudio.Services;

/// <summary>Defines methods for initializing and interacting with a SQLite database asynchronously.</summary>
/// <remarks>
/// Implementations provide database setup, SQL execution, and querying without blocking the UI thread.
/// </remarks>
public interface ISqliteService
{
    /// <summary>Initializes the database connection using the default database path.</summary>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    Task InitializeAsync();

    /// <summary>Initializes the database connection using the specified database file path.</summary>
    /// <param name="dbPath">The file path to the database to initialize.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    Task InitializeAsync(string dbPath);

    /// <summary>Ensures that the sample data exists in the underlying data store.</summary>
    /// <returns>A task that completes when the sample data has been verified or created.</returns>
    Task EnsureSampleDataAsync();

    /// <summary>Asynchronously retrieves a read-only list of all table names in the current database.</summary>
    /// <returns>A task containing the table names. The list is empty when no tables are present.</returns>
    Task<IReadOnlyList<string>> ListTablesAsync();

    /// <summary>Executes the specified SQL command asynchronously against the underlying database.</summary>
    /// <param name="sql">The SQL statement to execute. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous execution operation.</returns>
    Task ExecuteAsync(string sql);

    /// <summary>Executes the specified SQL query asynchronously and returns the result set.</summary>
    /// <param name="sql">The SQL query to execute. Cannot be null or empty.</param>
    /// <returns>A task containing rows returned by the query, or an empty list when no rows are returned.</returns>
    Task<IReadOnlyList<object>> QueryAsync(string sql);
}

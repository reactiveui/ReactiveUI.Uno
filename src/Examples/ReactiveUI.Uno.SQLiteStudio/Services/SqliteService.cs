// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using SQLite;

namespace ReactiveUI.Uno.SQLiteStudio.Services;

/// <summary>
/// Provides a thread-safe, singleton service for managing asynchronous SQLite database operations, including
/// initialization, table management, and query execution.
/// </summary>
/// <remarks>The SqliteService class offers a shared implementation of ISqliteService for use throughout an
/// application. It handles database connection lifecycle, ensures the database is properly initialized, and provides
/// convenience methods for executing SQL commands and queries asynchronously. The service is designed for use in
/// scenarios where a single, shared SQLite database is required, such as in mobile or desktop applications. All
/// operations are performed asynchronously to avoid blocking the calling thread.</remarks>
public sealed class SqliteService : ISqliteService
{
    /// <summary>Stores the lazy singleton SQLite service instance.</summary>
    private static readonly Lazy<ISqliteService> LazyInstance = new(() => new SqliteService());

    /// <summary>Stores the active SQLite connection.</summary>
    private SQLiteAsyncConnection? _connection;

    /// <summary>Initializes a new instance of the <see cref="SqliteService"/> class.</summary>
    private SqliteService()
    {
    }

    /// <summary>Gets the singleton instance of the SQLite service.</summary>
    /// <remarks>Use this property to access the shared implementation of the SQLite service throughout the
    /// application. The instance is initialized on first access and is thread-safe.</remarks>
    public static ISqliteService Instance => LazyInstance.Value;

    /// <summary>Initializes the database connection asynchronously, creating the database file if it does not exist.</summary>
    /// <remarks>If the connection is already initialized, this method returns immediately. The method ensures
    /// the target directory exists and configures the database for write-ahead logging and foreign key constraints
    /// where supported. Failures to set these options are ignored.</remarks>
    /// <param name="dbPath">The optional file path to the database. If null, a default path in the local application data folder is used.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    public async Task InitializeAsync(string? dbPath = null)
    {
        if (_connection is not null)
        {
            return;
        }

        dbPath ??= Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "app.db3");

        // Ensure folder exists
        var dir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            _ = Directory.CreateDirectory(dir);
        }

        const SQLiteOpenFlags flags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache;
        _connection = new(dbPath, flags);

        // Some platforms/providers will throw for this PRAGMA. Ignore failures.
        try
        {
            await _connection.ExecuteAsync("PRAGMA journal_mode=WAL;").ConfigureAwait(false);
        }
        catch (SQLiteException ex)
        {
            Debug.WriteLine($"SQLite journal mode pragma was not applied: {ex.Message}");
        }

        // Enable FK constraints where supported (ignore failures)
        try
        {
            await _connection.ExecuteAsync("PRAGMA foreign_keys=ON;").ConfigureAwait(false);
        }
        catch (SQLiteException ex)
        {
            Debug.WriteLine($"SQLite foreign key pragma was not applied: {ex.Message}");
        }
    }

    /// <summary>
    /// Ensures that the sample data exists in the Users table of the database. Creates the table and inserts default
    /// user records if none are present.
    /// </summary>
    /// <remarks>This method is typically used to initialize the database with sample data for development or
    /// testing purposes. If the Users table already contains data, no changes are made.</remarks>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task EnsureSampleDataAsync()
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        await conn.ExecuteAsync("CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, Name TEXT, Age INTEGER);").ConfigureAwait(false);

        var count = await conn.ExecuteScalarAsync<int>("SELECT COUNT(1) FROM Users;").ConfigureAwait(false);
        if (count != 0)
        {
            return;
        }

        await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Alice', 30);").ConfigureAwait(false);
        await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Bob', 42);").ConfigureAwait(false);
        await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Charlie', 25);").ConfigureAwait(false);
    }

    /// <summary>Asynchronously retrieves the names of all user-defined tables in the database.</summary>
    /// <remarks>System tables and tables with names starting with 'sqlite_' are excluded from the results.
    /// The operation does not include views or other database objects.</remarks>
    /// <returns>A read-only list of strings containing the names of all user-defined tables. The list is empty if no such tables
    /// exist.</returns>
    public async Task<IReadOnlyList<string>> ListTablesAsync()
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        var tables = await conn.QueryAsync<MasterRow>("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY 1").ConfigureAwait(false);
        var names = new List<string>(tables.Count);
        foreach (var table in tables)
        {
            if (!string.IsNullOrWhiteSpace(table.Name))
            {
                names.Add(table.Name);
            }
        }

        return names;
    }

    /// <summary>Executes the specified SQL command asynchronously against the database connection.</summary>
    /// <param name="sql">The SQL command to execute. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync(string sql)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        await conn.ExecuteAsync(sql).ConfigureAwait(false);
    }

    /// <summary>Executes the specified SQL query asynchronously and returns the results as a read-only list of objects.</summary>
    /// <remarks>The type and content of the returned objects depend on the SQL statement. For queries against
    /// the 'users' table, the list contains user row objects. For queries against 'sqlite_master', the list contains
    /// the names of the objects as strings. For other SELECT statements, the list contains the first column values as
    /// strings if possible. For non-SELECT statements, the list contains a status or error message. This method does
    /// not throw exceptions for SQL execution errors; instead, error messages are returned in the result
    /// list.</remarks>
    /// <param name="sql">The SQL statement to execute. This can be a SELECT or a non-SELECT statement. Cannot be null or whitespace.</param>
    /// <returns>A read-only list of objects representing the query results. For SELECT statements, the list contains the result
    /// rows or column values. For non-SELECT statements, the list contains a status message indicating the number of
    /// rows affected or an error message if execution fails. Returns an empty list if the SQL statement is null or
    /// whitespace.</returns>
    public async Task<IReadOnlyList<object>> QueryAsync(string sql)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(sql))
        {
            return [];
        }

        var lowered = sql.Trim().ToLowerInvariant();
        return lowered.StartsWith("select")
            ? await QuerySelectAsync(conn, sql, lowered).ConfigureAwait(false)
            : await ExecuteNonQueryAsync(conn, sql).ConfigureAwait(false);
    }

    /// <summary>Executes a SELECT statement and maps the result rows.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <param name="loweredSql">The lower-case SQL statement used for routing.</param>
    /// <returns>A task that returns the mapped query results.</returns>
    private static async Task<IReadOnlyList<object>> QuerySelectAsync(SQLiteAsyncConnection connection, string sql, string loweredSql)
    {
        if (loweredSql.Contains(" from users"))
        {
            return await QueryUsersAsync(connection, sql).ConfigureAwait(false);
        }

        return loweredSql.Contains("from sqlite_master")
            ? await QueryMasterAsync(connection, sql).ConfigureAwait(false)
            : await QueryScalarFallbackAsync(connection, sql).ConfigureAwait(false);
    }

    /// <summary>Queries sample user rows.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>A task that returns the user rows as objects.</returns>
    private static async Task<IReadOnlyList<object>> QueryUsersAsync(SQLiteAsyncConnection connection, string sql)
    {
        var rows = await connection.QueryAsync<UserRow>(sql).ConfigureAwait(false);
        var result = new List<object>(rows.Count);
        result.AddRange(rows);
        return result;
    }

    /// <summary>Queries sqlite_master rows.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>A task that returns the sqlite_master names as objects.</returns>
    private static async Task<IReadOnlyList<object>> QueryMasterAsync(SQLiteAsyncConnection connection, string sql)
    {
        var rows = await connection.QueryAsync<MasterRow>(sql).ConfigureAwait(false);
        var result = new List<object>(rows.Count);
        foreach (var row in rows)
        {
            result.Add(row.Name);
        }

        return result;
    }

    /// <summary>Queries scalar string values as a fallback for SELECT statements.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>A task that returns scalar values or a fallback status message.</returns>
    private static async Task<IReadOnlyList<object>> QueryScalarFallbackAsync(SQLiteAsyncConnection connection, string sql)
    {
        try
        {
            var scalarList = await connection.QueryScalarsAsync<string>(sql).ConfigureAwait(false);
            var result = new List<object>(scalarList.Count);
            result.AddRange(scalarList);
            return result;
        }
        catch (SQLiteException)
        {
            return ["Query executed."];
        }
    }

    /// <summary>Executes a non-query SQL statement and returns a status row.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>A task that returns a status row.</returns>
    private static async Task<IReadOnlyList<object>> ExecuteNonQueryAsync(SQLiteAsyncConnection connection, string sql)
    {
        try
        {
            var affected = await connection.ExecuteAsync(sql).ConfigureAwait(false);
            return [$"Rows affected: {affected}"];
        }
        catch (SQLiteException ex)
        {
            return [ex.Message];
        }
    }

    /// <summary>Gets the initialized SQLite connection.</summary>
    /// <returns>A task that returns the active SQLite connection.</returns>
    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is null)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        return _connection!;
    }

    /// <summary>Represents a row from sqlite_master.</summary>
    private sealed class MasterRow
    {
        /// <summary>Gets or sets the table name.</summary>
        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }

    /// <summary>Represents a sample user row.</summary>
    private sealed class UserRow
    {
        /// <summary>Gets or sets the user identifier.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the user name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the user age.</summary>
        public int Age { get; set; }

        public override string ToString() => $"{Id}: {Name} ({Age})";
    }
}

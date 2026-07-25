// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using SQLite;

namespace ReactiveUI.Uno.SQLiteStudio.Services;

/// <summary>Maps ad hoc SQLite query results into rows the example can display.</summary>
internal static class SqliteQueryMapper
{
    /// <summary>Executes a SELECT statement and maps the result rows.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <param name="loweredSql">The lower-case SQL statement used for routing.</param>
    /// <returns>A task that returns the mapped query results.</returns>
    internal static async Task<IReadOnlyList<object>> QuerySelectAsync(
        SQLiteAsyncConnection connection,
        string sql,
        string loweredSql)
    {
        if (loweredSql.Contains(" from users"))
        {
            return await QueryUsersAsync(connection, sql).ConfigureAwait(false);
        }

        return loweredSql.Contains("from sqlite_master")
            ? await QueryMasterAsync(connection, sql).ConfigureAwait(false)
            : await QueryScalarFallbackAsync(connection, sql).ConfigureAwait(false);
    }

    /// <summary>Executes a non-query SQL statement and returns a status row.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>A task that returns a status row.</returns>
    internal static async Task<IReadOnlyList<object>> ExecuteNonQueryAsync(
        SQLiteAsyncConnection connection,
        string sql)
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

    /// <summary>Queries sample user rows.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>A task that returns the user rows as objects.</returns>
    private static async Task<IReadOnlyList<object>> QueryUsersAsync(
        SQLiteAsyncConnection connection,
        string sql)
    {
        var rows = await connection.QueryAsync<SqliteService.UserRow>(sql).ConfigureAwait(false);
        var result = new List<object>(rows.Count);
        result.AddRange(rows);
        return result;
    }

    /// <summary>Queries sqlite_master rows.</summary>
    /// <param name="connection">The SQLite connection to use.</param>
    /// <param name="sql">The SQL statement to execute.</param>
    /// <returns>A task that returns the sqlite_master names as objects.</returns>
    private static async Task<IReadOnlyList<object>> QueryMasterAsync(
        SQLiteAsyncConnection connection,
        string sql)
    {
        var rows = await connection.QueryAsync<SqliteService.MasterRow>(sql).ConfigureAwait(false);
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
    private static async Task<IReadOnlyList<object>> QueryScalarFallbackAsync(
        SQLiteAsyncConnection connection,
        string sql)
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
}

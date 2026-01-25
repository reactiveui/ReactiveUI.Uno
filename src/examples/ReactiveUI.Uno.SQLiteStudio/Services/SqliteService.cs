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
    private static readonly Lazy<ISqliteService> LazyInstance = new(() => new SqliteService());
    private SQLiteAsyncConnection? _connection;

    private SqliteService()
    {
    }

    /// <summary>
    /// Gets the singleton instance of the SQLite service.
    /// </summary>
    /// <remarks>Use this property to access the shared implementation of the SQLite service throughout the
    /// application. The instance is initialized on first access and is thread-safe.</remarks>
    public static ISqliteService Instance => LazyInstance.Value;

    /// <summary>
    /// Initializes the database connection asynchronously, creating the database file if it does not exist.
    /// </summary>
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
            Directory.CreateDirectory(dir);
        }

        var flags = SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.SharedCache;
        _connection = new SQLiteAsyncConnection(dbPath, flags);

        // Some platforms/providers will throw for this PRAGMA. Ignore failures.
        try
        {
            await _connection.ExecuteAsync("PRAGMA journal_mode=WAL;").ConfigureAwait(false);
        }
        catch (SQLiteException)
        {
            // ignore
        }
        catch
        {
            // ignore
        }

        // Enable FK constraints where supported (ignore failures)
        try
        {
            await _connection.ExecuteAsync("PRAGMA foreign_keys=ON;").ConfigureAwait(false);
        }
        catch
        {
            // ignore
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
        if (count == 0)
        {
            await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Alice', 30);").ConfigureAwait(false);
            await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Bob', 42);").ConfigureAwait(false);
            await conn.ExecuteAsync("INSERT INTO Users (Name, Age) VALUES ('Charlie', 25);").ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Asynchronously retrieves the names of all user-defined tables in the database.
    /// </summary>
    /// <remarks>System tables and tables with names starting with 'sqlite_' are excluded from the results.
    /// The operation does not include views or other database objects.</remarks>
    /// <returns>A read-only list of strings containing the names of all user-defined tables. The list is empty if no such tables
    /// exist.</returns>
    public async Task<IReadOnlyList<string>> ListTablesAsync()
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        var tables = await conn.QueryAsync<MasterRow>("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY 1").ConfigureAwait(false);
        return tables.Select(x => x.name).Where(x => !string.IsNullOrWhiteSpace(x)).ToList()!;
    }

    /// <summary>
    /// Executes the specified SQL command asynchronously against the database connection.
    /// </summary>
    /// <param name="sql">The SQL command to execute. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task ExecuteAsync(string sql)
    {
        var conn = await GetConnectionAsync().ConfigureAwait(false);
        await conn.ExecuteAsync(sql).ConfigureAwait(false);
    }

    /// <summary>
    /// Executes the specified SQL query asynchronously and returns the results as a read-only list of objects.
    /// </summary>
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
            return Array.Empty<object>();
        }

        var lowered = sql.Trim().ToLowerInvariant();
        if (lowered.StartsWith("select"))
        {
            if (lowered.Contains(" from users"))
            {
                var rows = await conn.QueryAsync<UserRow>(sql).ConfigureAwait(false);
                return rows.Cast<object>().ToList();
            }
            if (lowered.Contains("from sqlite_master"))
            {
                var rows = await conn.QueryAsync<MasterRow>(sql).ConfigureAwait(false);

                // Render as string list
                return rows.ConvertAll(r => (object)r.name);
            }

            // Fallback: try to get first column as strings
            try
            {
                var scalarList = await conn.QueryScalarsAsync<string>(sql).ConfigureAwait(false);
                return scalarList.Cast<object>().ToList();
            }
            catch
            {
                // Last resort: execute without mapping
                return new List<object> { "Query executed." };
            }
        }
        else
        {
            // Non-SELECT: execute and return status
            try
            {
                var affected = await conn.ExecuteAsync(sql).ConfigureAwait(false);
                return new List<object> { $"Rows affected: {affected}" };
            }
            catch (Exception ex)
            {
                return new List<object> { ex.Message };
            }
        }
    }

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_connection is null)
        {
            await InitializeAsync().ConfigureAwait(false);
        }

        return _connection!;
    }

    private sealed class MasterRow
    {
        public string name { get; set; } = string.Empty;
    }

    private sealed class UserRow
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }

        public override string ToString() => $"{Id}: {Name} ({Age})";
    }
}

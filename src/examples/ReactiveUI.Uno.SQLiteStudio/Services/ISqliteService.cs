namespace ReactiveUI.Uno.SQLiteStudio.Services;

/// <summary>
/// Defines methods for initializing and interacting with a SQLite database asynchronously.
/// </summary>
/// <remarks>Implementations of this interface provide functionality for database setup, executing SQL commands,
/// and querying data in a SQLite database. All operations are performed asynchronously to support non-blocking database
/// access in applications.</remarks>
public interface ISqliteService
{
    /// <summary>
    /// Asynchronously initializes the database connection, optionally using the specified database file path.
    /// </summary>
    /// <param name="dbPath">The file path to the database to initialize. If null, the default database path is used.</param>
    /// <returns>A task that represents the asynchronous initialization operation.</returns>
    Task InitializeAsync(string? dbPath = null);

    /// <summary>
    /// Ensures that the sample data is present in the underlying data store, creating it if necessary.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task completes when the sample data has been verified or
    /// created.</returns>
    Task EnsureSampleDataAsync();

    /// <summary>
    /// Asynchronously retrieves a read-only list of all table names in the current database.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of strings, each
    /// representing the name of a table in the database. The list is empty if no tables are present.</returns>
    Task<IReadOnlyList<string>> ListTablesAsync();

    /// <summary>
    /// Executes the specified SQL command asynchronously against the underlying database.
    /// </summary>
    /// <param name="sql">The SQL statement to execute. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous execution operation.</returns>
    Task ExecuteAsync(string sql);

    /// <summary>
    /// Executes the specified SQL query asynchronously and returns the result set as a read-only list of objects.
    /// </summary>
    /// <param name="sql">The SQL query to execute. Cannot be null or empty.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of objects
    /// representing the rows returned by the query. The list is empty if the query returns no results.</returns>
    Task<IReadOnlyList<object>> QueryAsync(string sql);
}

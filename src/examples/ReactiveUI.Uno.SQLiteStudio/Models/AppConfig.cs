namespace ReactiveUI.Uno.SQLiteStudio.Models;

/// <summary>
/// Represents the application configuration settings.
/// </summary>
public record AppConfig
{
    /// <summary>
    /// Gets the name of the environment in which the application is running.
    /// </summary>
    public string? Environment { get; init; }
}

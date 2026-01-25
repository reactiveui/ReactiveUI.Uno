namespace ReactiveUI.Uno.SQLiteStudio.Models;

/// <summary>
/// Represents an immutable entity with a specified name.
/// </summary>
/// <param name="Name">The name that uniquely identifies the entity. Cannot be null.</param>
public record Entity(string Name);

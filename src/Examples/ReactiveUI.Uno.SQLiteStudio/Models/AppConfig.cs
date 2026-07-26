// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.SQLiteStudio.Models;

/// <summary>Represents the application configuration settings.</summary>
public sealed record AppConfig
{
    /// <summary>Gets the name of the environment in which the application is running.</summary>
    public string? Environment { get; init; }
}

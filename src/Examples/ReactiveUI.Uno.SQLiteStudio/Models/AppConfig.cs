// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.SQLiteStudio.Models;

/// <summary>Represents the application configuration settings.</summary>
public sealed record AppConfig
{
    /// <summary>Gets the name of the environment in which the application is running.</summary>
    public string? Environment { get; init; }
}

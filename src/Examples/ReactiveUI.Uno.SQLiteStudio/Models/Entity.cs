// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Uno.SQLiteStudio.Models;

/// <summary>Represents an immutable entity with a specified name.</summary>
/// <param name="Name">The name that uniquely identifies the entity. Cannot be null.</param>
public sealed record Entity(string Name);

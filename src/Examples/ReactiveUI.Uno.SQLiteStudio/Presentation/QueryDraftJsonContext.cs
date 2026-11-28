// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace ReactiveUI.Uno.SQLiteStudio.Presentation;

/// <summary>Provides the JSON contract for the query editor's saved draft.</summary>
[JsonSerializable(typeof(string))]
internal sealed partial class QueryDraftJsonContext : JsonSerializerContext;

// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Text.Json.Serialization;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>JSON serialization context for AOT-safe serialization.</summary>
[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(TestState))]
[JsonSerializable(typeof(NestedTestState))]
internal sealed partial class TestStateJsonContext : JsonSerializerContext;

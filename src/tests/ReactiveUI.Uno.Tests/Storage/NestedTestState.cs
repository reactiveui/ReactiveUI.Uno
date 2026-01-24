// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>
/// Nested test state for complex serialization tests.
/// </summary>
[DataContract]
public class NestedTestState
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the inner state.
    /// </summary>
    [DataMember]
    public TestState? Inner { get; set; }
}

// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>
/// Test state class for serialization tests.
/// </summary>
[DataContract]
public class TestState
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    [DataMember]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [DataMember]
    public int Value { get; set; }
}

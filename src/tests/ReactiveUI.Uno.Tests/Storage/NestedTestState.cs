// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>Nested test state for complex serialization tests.</summary>
[System.Diagnostics.DebuggerDisplay("NestedTestState: {Id}")]
[DataContract]
public class NestedTestState
{
    /// <summary>Gets or sets the identifier.</summary>
    [DataMember]
    public int Id { get; set; }

    /// <summary>Gets or sets the inner state.</summary>
    [DataMember]
    public TestState? Inner { get; set; }
}

// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>Contains round-trip and XML parsing tests for the WinRT application data driver.</summary>
public partial class WinRTAppDataDriverTests
{
    /// <summary>Validates that XML state can be saved and loaded when application storage is available.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateThenLoadState_RoundTripsXmlState_WhenApplicationStorageIsAvailable()
    {
        var state = new TestState { Name = "RoundTrip", Value = DefaultStateValue };

        try
        {
            await _sut.SaveState(state).ToTask();
            var loaded = await _sut.LoadState().ToTask();

            await Assert.That(loaded).IsNotNull();
            await Assert.That(loaded).IsAssignableTo<TestState>();
            var loadedState = (TestState)loaded!;
            await Assert.That(loadedState.Name).IsEqualTo(state.Name);
            await Assert.That(loadedState.Value).IsEqualTo(state.Value);
        }
        catch (Exception ex) when (IsApplicationStorageUnavailable(ex))
        {
            SkipUnavailableApplicationStorage(ex);
        }
    }

    /// <summary>Validates that XML state parsing preserves type names with Unix line endings.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task ParseXmlishState_WithLfHeader_PreservesFullTypeName()
    {
        var typeName = typeof(TestState).AssemblyQualifiedName!;
        var (parsedTypeName, xml) = WinRTAppDataDriver.ParseXmlishState($"{typeName}\n{XmlStatePayload}");

        await Assert.That(parsedTypeName).IsEqualTo(typeName);
        await Assert.That(xml).IsEqualTo(XmlStatePayload);
    }

    /// <summary>Validates that XML state parsing preserves type names with Windows line endings.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task ParseXmlishState_WithCrLfHeader_PreservesFullTypeName()
    {
        var typeName = typeof(TestState).AssemblyQualifiedName!;
        var (parsedTypeName, xml) = WinRTAppDataDriver.ParseXmlishState($"{typeName}\r\n{XmlStatePayload}");

        await Assert.That(parsedTypeName).IsEqualTo(typeName);
        await Assert.That(xml).IsEqualTo(XmlStatePayload);
    }

    /// <summary>Validates that XML state parsing rejects content without a type header.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task ParseXmlishState_WithoutHeader_ThrowsInvalidDataException() =>
        await Assert.That(() => WinRTAppDataDriver.ParseXmlishState(XmlStatePayload)).Throws<InvalidDataException>();

    /// <summary>Validates that XML state parsing rejects an empty type header.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task ParseXmlishState_WithEmptyHeader_ThrowsInvalidDataException() =>
        await Assert.That(() => WinRTAppDataDriver.ParseXmlishState($"\n{XmlStatePayload}"))
            .Throws<InvalidDataException>();

    /// <summary>Validates that JSON state can be saved and loaded when application storage is available.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateThenLoadStateWithTypeInfo_RoundTripsJsonState_WhenApplicationStorageIsAvailable()
    {
        var state = new TestState { Name = "JsonRoundTrip", Value = JsonRoundTripValue };

        try
        {
            await _sut.SaveState(state, TestStateJsonContext.Default.TestState).ToTask();
            var loaded = await _sut.LoadState(TestStateJsonContext.Default.TestState).ToTask();

            await Assert.That(loaded).IsNotNull();
            await Assert.That(loaded!.Name).IsEqualTo(state.Name);
            await Assert.That(loaded.Value).IsEqualTo(state.Value);
        }
        catch (Exception ex) when (IsApplicationStorageUnavailable(ex))
        {
            SkipUnavailableApplicationStorage(ex);
        }
    }

    /// <summary>Validates that invalidating state covers both existing and missing persisted files.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_Twice_DeletesExistingFilesThenIgnoresMissingFiles()
    {
        var state = new TestState { Name = "Invalidate", Value = InvalidationStateValue };

        try
        {
            await _sut.SaveState(state).ToTask();
            await _sut.SaveState(state, TestStateJsonContext.Default.TestState).ToTask();
            await _sut.InvalidateState().ToTask();
            await _sut.InvalidateState().ToTask();

            await Assert.That(_sut.InvalidateState()).IsNotNull();
        }
        catch (Exception ex) when (IsApplicationStorageUnavailable(ex))
        {
            SkipUnavailableApplicationStorage(ex);
        }
    }
}

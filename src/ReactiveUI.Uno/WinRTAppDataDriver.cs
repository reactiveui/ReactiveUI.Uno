// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Loads and saves state to persistent storage.</summary>
public class WinRTAppDataDriver : ISuspensionDriver
{
    /// <summary>The JSON state file name.</summary>
    private const string JsonStateFileName = "appData.json";

    /// <summary>The data-contract state file name, written by <c>WinRTAppDataDriverUnsafe</c> in ReactiveUI.Uno.Unsafe.</summary>
    private const string XmlStateFileName = "appData.xmlish";

    /// <inheritdoc/>
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);

        return Observable.FromAsync(
        async () =>
        {
            var file = await ApplicationData.Current.RoamingFolder.GetFileAsync(JsonStateFileName);
            var json = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);

            return JsonSerializer.Deserialize(json, typeInfo);
        });
    }

    /// <inheritdoc/>
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(typeInfo);

        return Observable.FromAsync(
        async () =>
        {
            var json = JsonSerializer.Serialize(state, typeInfo);

            var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(
                JsonStateFileName,
                CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, json, UnicodeEncoding.Utf8);

            return Unit.Default;
        });
    }

    /// <inheritdoc/>
    /// <remarks>Deletes the JSON state and any data-contract state saved by the unsafe driver.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<Unit> InvalidateState() =>
        Observable.FromAsync(
        static async () =>
        {
            var folder = ApplicationData.Current.RoamingFolder;

            await DeleteIfPresentAsync(folder, XmlStateFileName);
            await DeleteIfPresentAsync(folder, JsonStateFileName);

            return Unit.Default;
        });

    /// <summary>Deletes a state file when it exists.</summary>
    /// <param name="folder">The storage folder containing the state file.</param>
    /// <param name="fileName">The state file name.</param>
    /// <returns>A task that completes after the file has been deleted or found to be absent.</returns>
    private static async Task DeleteIfPresentAsync(StorageFolder folder, string fileName)
    {
        try
        {
            var file = await folder.GetFileAsync(fileName);
            await file.DeleteAsync();
        }
        catch (FileNotFoundException)
        {
            // File does not exist, so there is no state to invalidate.
        }
    }
}

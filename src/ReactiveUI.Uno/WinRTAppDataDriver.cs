// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Text;
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

    /// <summary>The data-contract state file name.</summary>
    private const string XmlStateFileName = "appData.xmlish";

    /// <inheritdoc/>
    [RequiresDynamicCode("LoadState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("LoadState implementations may use serialization which may require unreferenced code")]
    public IObservable<object?> LoadState() => Observable.FromAsync(
    static async () =>
    {
        var x = await ApplicationData.Current.RoamingFolder.GetFileAsync(XmlStateFileName);
        var t = await FileIO.ReadTextAsync(x, UnicodeEncoding.Utf8);

        var (typeName, xml) = ParseXmlishState(t);
        var serializer = new DataContractSerializer(Type.GetType(typeName, throwOnError: true)!);

        // NB: WinRT is terrible
        return serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
    });

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
    [RequiresDynamicCode("SaveState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("SaveState implementations may use serialization which may require unreferenced code")]
    public IObservable<Unit> SaveState<T>(T state) => Observable.FromAsync(
    async () =>
    {
        ArgumentNullException.ThrowIfNull(state);

        await using var ms = new MemoryStream();
        await using var writer = new StreamWriter(ms, Encoding.UTF8);
        var serializer = new DataContractSerializer(state.GetType());
        await writer.WriteLineAsync(state.GetType().AssemblyQualifiedName);
        await writer.FlushAsync();

        serializer.WriteObject(ms, state);

        var x = await ApplicationData.Current.RoamingFolder.CreateFileAsync(
            XmlStateFileName,
            CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteBytesAsync(x, ms.ToArray());

        return Unit.Default;
    });

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
    public IObservable<Unit> InvalidateState() =>
        Observable.FromAsync(
        static async () =>
        {
            var folder = ApplicationData.Current.RoamingFolder;

            await DeleteIfPresentAsync(folder, XmlStateFileName);
            await DeleteIfPresentAsync(folder, JsonStateFileName);

            return Unit.Default;
        });

    /// <summary>Splits the persisted XML state into the saved type name and XML payload.</summary>
    /// <param name="content">The persisted XML state content.</param>
    /// <returns>The saved type name and XML payload.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="content"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidDataException">Thrown when the persisted state header is missing or empty.</exception>
    internal static (string TypeName, string Xml) ParseXmlishState(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        var line = content.IndexOf('\n');
        if (line < 0)
        {
            throw new InvalidDataException("Persisted state is missing a type header.");
        }

        var typeName = content[..line].TrimEnd('\r');
        if (string.IsNullOrWhiteSpace(typeName))
        {
            throw new InvalidDataException("Persisted state has an empty type header.");
        }

        return (typeName, content[(line + 1)..]);
    }

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

// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
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
    /// <inheritdoc/>
    [RequiresDynamicCode("LoadState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("LoadState implementations may use serialization which may require unreferenced code")]
    public IObservable<object?> LoadState() => Observable.FromAsync(
    async () =>
    {
        var x = await ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish");
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

        return Observable.FromAsync<T?>(
        async () =>
        {
            var file = await ApplicationData.Current.RoamingFolder.GetFileAsync("appData.json");
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

        try
        {
            await using var ms = new MemoryStream();
            await using var writer = new StreamWriter(ms, Encoding.UTF8);
            var serializer = new DataContractSerializer(state.GetType());
            await writer.WriteLineAsync(state.GetType().AssemblyQualifiedName);
            await writer.FlushAsync();

            serializer.WriteObject(ms, state);

            var x = await ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.xmlish", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteBytesAsync(x, ms.ToArray());
        }
        catch (Exception)
        {
            throw;
        }

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

            var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, json, UnicodeEncoding.Utf8);

            return Unit.Default;
        });
    }

    /// <inheritdoc/>
    public IObservable<Unit> InvalidateState() =>
        Observable.FromAsync(
        async () =>
        {
            var folder = ApplicationData.Current.RoamingFolder;

            // Delete xmlish file (used by DataContract serialization)
            try
            {
                var xmlFile = await folder.GetFileAsync("appData.xmlish");
                await xmlFile.DeleteAsync();
            }
            catch (FileNotFoundException)
            {
                // File doesn't exist, nothing to invalidate
            }

            // Delete json file (used by JSON serialization)
            try
            {
                var jsonFile = await folder.GetFileAsync("appData.json");
                await jsonFile.DeleteAsync();
            }
            catch (FileNotFoundException)
            {
                // File doesn't exist, nothing to invalidate
            }

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
}

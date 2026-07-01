// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using ReactiveUI.Primitives;
using RxObservable = System.Reactive.Linq.Observable;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace ReactiveUI.Uno;

/// <summary>
/// Loads and saves state to persistent storage.
/// </summary>
public class WinRTAppDataDriver : ISuspensionDriver
{
    /// <inheritdoc/>
    [RequiresDynamicCode("LoadState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("LoadState implementations may use serialization which may require unreferenced code")]
    public IObservable<object?> LoadState() => RxObservable.StartAsync(
    async () =>
    {
        var x = await ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish");
        var t = await FileIO.ReadTextAsync(x, UnicodeEncoding.Utf8);

        var line = t.IndexOf('\n');
        var typeName = t.Substring(0, line - 1); // -1 for CR
        var serializer = new DataContractSerializer(Type.GetType(typeName!)!);

        // NB: WinRT is terrible
        return serializer?.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(t.Substring(line + 1))));
    },
    TaskPoolScheduler.Default);

    /// <inheritdoc/>
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(typeInfo);

        return RxObservable.StartAsync<T?>(
        async () =>
        {
            var file = await ApplicationData.Current.RoamingFolder.GetFileAsync("appData.json");
            var json = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);

            return JsonSerializer.Deserialize(json, typeInfo);
        },
        TaskPoolScheduler.Default);
    }

    /// <inheritdoc/>
    [RequiresDynamicCode("SaveState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("SaveState implementations may use serialization which may require unreferenced code")]
    public IObservable<RxVoid> SaveState<T>(T state) => RxObservable.StartAsync(
    async () =>
    {
        ArgumentNullException.ThrowIfNull(state);

        try
        {
            await using (var ms = new MemoryStream())
            await using (var writer = new StreamWriter(ms, Encoding.UTF8))
            {
                var serializer = new DataContractSerializer(state.GetType());
                writer.WriteLine(state.GetType().AssemblyQualifiedName);
                writer.Flush();

                serializer.WriteObject(ms, state);

                var x = await ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.xmlish", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(x, ms.ToArray());
            }
        }
        catch (Exception)
        {
            throw;
        }

        return RxVoid.Default;
    },
    TaskPoolScheduler.Default);

    /// <inheritdoc/>
    public IObservable<RxVoid> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(typeInfo);

        return RxObservable.StartAsync(
        async () =>
        {
            var json = JsonSerializer.Serialize(state, typeInfo);

            var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.json", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, json, UnicodeEncoding.Utf8);

            return RxVoid.Default;
        },
        TaskPoolScheduler.Default);
    }

    /// <inheritdoc/>
    public IObservable<RxVoid> InvalidateState() =>
        RxObservable.StartAsync(
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

            return RxVoid.Default;
        },
        TaskPoolScheduler.Default);
}

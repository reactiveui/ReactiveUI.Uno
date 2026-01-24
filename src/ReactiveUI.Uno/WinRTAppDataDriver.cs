// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization.Metadata;
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
    public IObservable<object?> LoadState() => Observable.StartAsync<object?>(
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
    RxSchedulers.TaskpoolScheduler);

    /// <inheritdoc/>
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo)
    {
        // TODO: Fix this method to actually do something useful
        return default!;
    }

    /// <inheritdoc/>
    [RequiresDynamicCode("SaveState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("SaveState implementations may use serialization which may require unreferenced code")]
    public IObservable<Unit> SaveState<T>(T state) => Observable.StartAsync(
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
    },
    RxSchedulers.TaskpoolScheduler);

    /// <inheritdoc/>
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo)
    {
        // TODO: Fix this method to actually do something useful
        return default!;
    }

    /// <inheritdoc/>
    public IObservable<Unit> InvalidateState() =>
        ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish").AsTask().ToObservable()
                       .SelectMany(x => x.DeleteAsync().AsTask().ToObservable());
}

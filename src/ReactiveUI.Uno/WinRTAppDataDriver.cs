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
    public IObservable<object> LoadState() =>
        ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish").AsTask().ToObservable()
                       .SelectMany(x => FileIO.ReadTextAsync(x, UnicodeEncoding.Utf8).AsTask())
                       .SelectMany(x =>
                       {
                           var line = x.IndexOf('\n');
                           var typeName = x.Substring(0, line - 1); // -1 for CR
                           var serializer = new DataContractSerializer(Type.GetType(typeName!)!);

                           // NB: WinRT is terrible
                           var obj = serializer?.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(x.Substring(line + 1))));
                           return Observable.Return(obj!);
                       });

    /// <inheritdoc/>
    [RequiresDynamicCode("SaveState implementations may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("SaveState implementations may use serialization which may require unreferenced code")]
    public IObservable<Unit> SaveState(object state)
    {
        ArgumentNullException.ThrowIfNull(state);

        try
        {
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.UTF8);
            var serializer = new DataContractSerializer(state.GetType());
            writer.WriteLine(state.GetType().AssemblyQualifiedName);
            writer.Flush();

            serializer.WriteObject(ms, state);

            return ApplicationData.Current.RoamingFolder.CreateFileAsync("appData.xmlish", CreationCollisionOption.ReplaceExisting).AsTask().ToObservable()
                                  .SelectMany(x => FileIO.WriteBytesAsync(x, ms.ToArray()).AsTask().ToObservable());
        }
        catch (Exception ex)
        {
            return Observable.Throw<Unit>(ex);
        }
    }

    /// <inheritdoc/>
    [RequiresDynamicCode("InvalidateState uses JsonSerializer which requires dynamic code generation")]
    [RequiresUnreferencedCode("InvalidateState uses JsonSerializer which may require unreferenced code")]
    public IObservable<Unit> InvalidateState() =>
        ApplicationData.Current.RoamingFolder.GetFileAsync("appData.xmlish").AsTask().ToObservable()
                       .SelectMany(x => x.DeleteAsync().AsTask().ToObservable());
}

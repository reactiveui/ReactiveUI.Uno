// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Loads and saves state to persistent storage, including reflection-based data-contract state.</summary>
/// <remarks>
/// Use <see cref="WinRTAppDataDriver"/> when the app serializes through <see cref="JsonTypeInfo{T}"/> only. This driver
/// adds <see cref="SaveState{T}(T)"/> and <see cref="LoadState()"/>, which use <see cref="DataContractSerializer"/> and
/// so are not safe to trim or to compile ahead of time.
/// </remarks>
[DebuggerDisplay("WinRTAppDataDriverUnsafe")]
public sealed class WinRTAppDataDriverUnsafe : IUnsafeSuspensionDriver
{
    /// <summary>The data-contract state file name.</summary>
    private const string XmlStateFileName = "appData.xmlish";

    /// <summary>The AOT-safe driver that handles the typed operations.</summary>
    private readonly WinRTAppDataDriver _inner = new();

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer LoadState<T>(JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    public IObservable<object?> LoadState() => Observable.FromAsync(
    static async () =>
    {
        var file = await ApplicationData.Current.RoamingFolder.GetFileAsync(XmlStateFileName);
        var text = await FileIO.ReadTextAsync(file, UnicodeEncoding.Utf8);

        var (typeName, xml) = ParseXmlishState(text);
        var serializer = new DataContractSerializer(Type.GetType(typeName, throwOnError: true)!);

        return serializer.ReadObject(new MemoryStream(Encoding.UTF8.GetBytes(xml)));
    });

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<T?> LoadState<T>(JsonTypeInfo<T> typeInfo) => _inner.LoadState(typeInfo);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [RequiresUnreferencedCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
    [RequiresDynamicCode(
        "Implementations commonly use reflection-based serialization. "
        + "Prefer SaveState<T>(T, JsonTypeInfo<T>) for trimming or AOT scenarios.")]
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

        var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync(
            XmlStateFileName,
            CreationCollisionOption.ReplaceExisting);
        await FileIO.WriteBytesAsync(file, ms.ToArray());

        return Unit.Default;
    });

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<Unit> SaveState<T>(T state, JsonTypeInfo<T> typeInfo) => _inner.SaveState(state, typeInfo);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IObservable<Unit> InvalidateState() => _inner.InvalidateState();

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

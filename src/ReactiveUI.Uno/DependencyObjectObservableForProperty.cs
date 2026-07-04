// Copyright (c) 2021 - 2026 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Splat;

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Creates a observable for a property if available that is based on a DependencyProperty.</summary>
public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public int GetAffinityForObject(Type? type, string propertyName) => GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        if (type is null)
        {
            return 0;
        }

        if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
        {
            return 0;
        }

        return GetDependencyPropertyFetcher(type, propertyName) is null ? 0 : 6;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged, bool suppressWarnings)
    {
        ArgumentNullException.ThrowIfNull(sender);

        if (sender is not DependencyObject depSender)
        {
            throw new ArgumentException("The sender must be a DependencyObject", nameof(sender));
        }

        var type = sender.GetType();

        if (beforeChanged)
        {
            this.Log().Warn(
                CultureInfo.InvariantCulture,
                "Tried to bind DO {0}.{1}, but DPs can't do beforeChanged. Binding as POCO object",
                type.FullName,
                propertyName);

            var ret = new POCOObservableForProperty();
            return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged, suppressWarnings)
                .Select(x => new ObservedChange<object, object>(x.Sender, x.Expression, x.Value!));
        }

        var dependencyPropertyFetcher = GetDependencyPropertyFetcher(type, propertyName);
        if (dependencyPropertyFetcher is null)
        {
            this.Log().Warn(
                CultureInfo.InvariantCulture,
                "Tried to bind DO {0}.{1}, but DP doesn't exist. Binding as POCO object",
                type.FullName,
                propertyName);

            var ret = new POCOObservableForProperty();
            return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged, suppressWarnings)
                .Select(x => new ObservedChange<object, object>(x.Sender, x.Expression, x.Value!));
        }

        return Observable.Create<IObservedChange<object, object>>(subj =>
        {
            var handler = new DependencyPropertyChangedCallback((_, _) =>
                subj.OnNext(new ObservedChange<object, object>(sender, expression, default!)));

            var dependencyProperty = dependencyPropertyFetcher();
            var token = depSender.RegisterPropertyChangedCallback(dependencyProperty, handler);
            return Disposable.Create(() => depSender.UnregisterPropertyChangedCallback(dependencyProperty, token));
        });
    }

    /// <summary>Finds a static dependency property accessor declared on the supplied type or one of its base types.</summary>
    /// <param name="typeInfo">The type to inspect.</param>
    /// <param name="propertyName">The dependency property accessor name.</param>
    /// <returns>The matching property when found; otherwise, null.</returns>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    private static PropertyInfo? ActuallyGetProperty(TypeInfo typeInfo, string propertyName)
    {
        var current = typeInfo;
        while (current is not null)
        {
            var ret = current.GetDeclaredProperty(propertyName);
            if (ret?.IsStatic() == true)
            {
                return ret;
            }

            current = current.BaseType?.GetTypeInfo();
        }

        return null;
    }

    /// <summary>Finds a static dependency field declared on the supplied type or one of its base types.</summary>
    /// <param name="typeInfo">The type to inspect.</param>
    /// <param name="propertyName">The dependency field name.</param>
    /// <returns>The matching field when found; otherwise, null.</returns>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    private static FieldInfo? ActuallyGetField(TypeInfo typeInfo, string propertyName)
    {
        var current = typeInfo;
        while (current is not null)
        {
            var ret = current.GetDeclaredField(propertyName);
            if (ret?.IsStatic == true)
            {
                return ret;
            }

            current = current.BaseType?.GetTypeInfo();
        }

        return null;
    }

    /// <summary>Creates a dependency property fetcher for the supplied CLR property name.</summary>
    /// <param name="type">The dependency object type to inspect.</param>
    /// <param name="propertyName">The CLR property name.</param>
    /// <returns>A dependency property fetcher when the backing dependency property exists; otherwise, null.</returns>
    [RequiresUnreferencedCode("The method uses reflection and may not work in AOT environments.")]
    private static Func<DependencyProperty>? GetDependencyPropertyFetcher(Type type, string propertyName)
    {
        var typeInfo = type.GetTypeInfo();

        // Look for the DependencyProperty attached to this property name
        var pi = ActuallyGetProperty(typeInfo, propertyName + "Property");
        if (pi is not null)
        {
            var value = pi.GetValue(null);

            return value is null ? null : () => (DependencyProperty)value;
        }

        var fi = ActuallyGetField(typeInfo, propertyName + "Property");
        if (fi is not null)
        {
            var value = fi.GetValue(null);

            return value is null ? null : () => (DependencyProperty)value;
        }

        return null;
    }
}

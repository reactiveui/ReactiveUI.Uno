// Copyright (c) 2019-2026 ReactiveUI and Contributors. All rights reserved.
// ReactiveUI and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Splat;
#if REACTIVE_SHIM
using ReactiveUI.Uno.Reactive.Internal;
#else
using ReactiveUI.Uno.Internal;
#endif

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>Creates a observable for a property if available that is based on a DependencyProperty.</summary>
[RequiresUnreferencedCode("Uses reflection to find the DependencyProperty static fields and properties.")]
public class DependencyObjectObservableForProperty : ICreatesObservableForProperty, IEnableLogger
{
    /// <summary>The affinity assigned when a dependency property is available.</summary>
    private const int DependencyPropertyAffinity = 6;

    /// <summary>Returns the dependency-object affinity when the type declares a <c>{propertyName}Property</c> static member.</summary>
    /// <param name="type">The type that owns the property.</param>
    /// <param name="propertyName">The property name, without the <c>Property</c> suffix.</param>
    /// <param name="beforeChanged">Ignored; before-change requests fall back to plain object observation.</param>
    /// <returns>The dependency-object affinity for a <see cref="DependencyObject"/> type with such a member; otherwise zero.</returns>
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged)
    {
        if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
        {
            return 0;
        }

        return GetDependencyPropertyFetcher(type, propertyName) is null ? 0 : DependencyPropertyAffinity;
    }

    /// <summary>Returns an observable that raises whenever the dependency property changes on <paramref name="sender"/>.</summary>
    /// <param name="sender">The <see cref="DependencyObject"/> to observe.</param>
    /// <param name="expression">The expression carried on each notification.</param>
    /// <param name="propertyName">The property name, without the <c>Property</c> suffix.</param>
    /// <param name="beforeChanged"><see langword="true"/> observes the property as a plain object property, since a dependency property has no before-change notification.</param>
    /// <param name="suppressWarnings">Passed through to the plain-object observer when it is used.</param>
    /// <returns>An observable that registers a property-changed callback and unregisters it on disposal.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="sender"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="sender"/> is not a <see cref="DependencyObject"/>.</exception>
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged,
        bool suppressWarnings)
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

            return new POCOObservableForProperty()
                .GetNotificationForProperty(sender, expression, propertyName, beforeChanged, suppressWarnings);
        }

        var dependencyPropertyFetcher = GetDependencyPropertyFetcher(type, propertyName);
        if (dependencyPropertyFetcher is null)
        {
            this.Log().Warn(
                CultureInfo.InvariantCulture,
                "Tried to bind DO {0}.{1}, but DP doesn't exist. Binding as POCO object",
                type.FullName,
                propertyName);

            return new POCOObservableForProperty()
                .GetNotificationForProperty(sender, expression, propertyName, beforeChanged, suppressWarnings);
        }

        return ObservableFactory.CreateWithState<
            IObservedChange<object, object?>,
            (object Sender, DependencyObject DependencyObject, Expression Expression, Func<DependencyProperty> Fetcher)>(
            (sender, depSender, expression, dependencyPropertyFetcher),
            static (state, observer) =>
        {
            var handler = new DependencyPropertyChangedCallback((_, _) =>
                observer.OnNext(new ObservedChange<object, object?>(state.Sender, state.Expression, default)));

            var dependencyProperty = state.Fetcher();
            var token = state.DependencyObject.RegisterPropertyChangedCallback(dependencyProperty, handler);
            return Disposable.Create(
                (state.DependencyObject, Property: dependencyProperty, Token: token),
                static subscription => subscription.DependencyObject.UnregisterPropertyChangedCallback(
                    subscription.Property,
                    subscription.Token));
        });
    }

    /// <summary>Finds a static dependency property accessor on the supplied type or a base type.</summary>
    /// <param name="typeInfo">The type to inspect.</param>
    /// <param name="propertyName">The dependency property accessor name.</param>
    /// <returns>The matching property when found; otherwise, null.</returns>
    private static PropertyInfo? ActuallyGetProperty(TypeInfo typeInfo, string propertyName)
    {
        var current = typeInfo;
        while (current is not null)
        {
            var ret = current.GetDeclaredProperty(propertyName);
            if (ret?.GetMethod?.IsStatic == true)
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
    private static Func<DependencyProperty>? GetDependencyPropertyFetcher(Type type, string propertyName)
    {
        var typeInfo = type.GetTypeInfo();

        // Look for the DependencyProperty attached to this property name
        var pi = ActuallyGetProperty(typeInfo, $"{propertyName}Property");
        if (pi is not null)
        {
            var value = pi.GetValue(null);

            return value is null ? null : () => (DependencyProperty)value;
        }

        var fi = ActuallyGetField(typeInfo, $"{propertyName}Property");
        if (fi is not null)
        {
            var value = fi.GetValue(null);

            return value is null ? null : () => (DependencyProperty)value;
        }

        return null;
    }
}

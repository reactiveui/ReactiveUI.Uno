// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM

namespace ReactiveUI.Uno.Reactive;
#else

namespace ReactiveUI.Uno;
#endif

/// <summary>
/// Registers platform-specific services, type converters, and binding hooks required for ReactiveUI to operate on the
/// Uno platform.
/// </summary>
/// <remarks>This class is typically used internally by the ReactiveUI Uno integration to ensure that all
/// necessary services and type converters are available at application startup. It is not intended to be used directly
/// by application code.</remarks>
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        ArgumentNullException.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());
        registrar.RegisterConstant<IActivationForViewFetcher>(static () => new ActivationForViewFetcher());
        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new DependencyObjectObservableForProperty());

        registrar.RegisterConstant<IBindingTypeConverter>(static () => new StringConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new ByteToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableByteToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new ShortToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableShortToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new IntegerToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableIntegerToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new LongToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableLongToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new SingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableSingleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDoubleToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new DecimalToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new NullableDecimalToStringTypeConverter());
        registrar.RegisterConstant<IBindingTypeConverter>(static () => new BooleanToVisibilityTypeConverter());

        registrar.RegisterConstant<IPropertyBindingHook>(static () => new AutoDataTemplateBindingHook());
        registrar.RegisterConstant<ISuspensionDriver>(static () => new WinRTAppDataDriver());

        // Disables ViewCommand binding messages on Uno platform
        RxSchedulers.SuppressViewCommandBindingMessage = true;
    }
}

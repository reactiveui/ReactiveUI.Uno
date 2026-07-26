// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>Contains metadata tests for the WinRT application data driver.</summary>
public partial class WinRTAppDataDriverTests
{
    /// <summary>Validates that SaveState generic method exists and is accessible.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_GenericMethod_Exists()
    {
        var methods = typeof(WinRTAppDataDriver).GetMethods()
            .Where(m => m.Name == nameof(WinRTAppDataDriver.SaveState) && m.IsGenericMethod)
            .ToList();

        await Assert.That(methods.Count).IsGreaterThanOrEqualTo(1);
    }

    /// <summary>Validates that SaveState with JsonTypeInfo method exists.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_Method_Exists()
    {
        var methods = typeof(WinRTAppDataDriver).GetMethods()
            .Where(m => m.Name == nameof(WinRTAppDataDriver.SaveState))
            .ToList();

        await Assert.That(methods.Count).IsGreaterThanOrEqualTo(ExpectedOverloadCount);
    }

    /// <summary>Validates that LoadState non-generic method exists.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_NonGenericMethod_Exists()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(
            nameof(WinRTAppDataDriver.LoadState),
            Type.EmptyTypes);

        await Assert.That(method).IsNotNull();
    }

    /// <summary>Validates that LoadState with JsonTypeInfo method exists.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_Method_Exists()
    {
        var methods = typeof(WinRTAppDataDriver).GetMethods()
            .Where(m => m.Name == nameof(WinRTAppDataDriver.LoadState))
            .ToList();

        await Assert.That(methods.Count).IsGreaterThanOrEqualTo(ExpectedOverloadCount);
    }

    /// <summary>Validates that InvalidateState method exists.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_Method_Exists()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(nameof(WinRTAppDataDriver.InvalidateState));

        await Assert.That(method).IsNotNull();
    }

    /// <summary>Validates that SaveState method has RequiresDynamicCode attribute.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HasRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethods()
            .FirstOrDefault(m => m.Name == nameof(WinRTAppDataDriver.SaveState) && m.GetParameters().Length == 1);

        var attribute = method?
            .GetCustomAttributes(typeof(RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>Validates that SaveState method has RequiresUnreferencedCode attribute.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveState_HasRequiresUnreferencedCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethods()
            .FirstOrDefault(m => m.Name == nameof(WinRTAppDataDriver.SaveState) && m.GetParameters().Length == 1);

        var attribute = method?
            .GetCustomAttributes(typeof(RequiresUnreferencedCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>Validates that LoadState method has RequiresDynamicCode attribute.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_HasRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(
            nameof(WinRTAppDataDriver.LoadState),
            Type.EmptyTypes);

        var attribute = method?
            .GetCustomAttributes(typeof(RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>Validates that LoadState method has RequiresUnreferencedCode attribute.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadState_HasRequiresUnreferencedCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(
            nameof(WinRTAppDataDriver.LoadState),
            Type.EmptyTypes);

        var attribute = method?
            .GetCustomAttributes(typeof(RequiresUnreferencedCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNotNull();
    }

    /// <summary>Validates that typed SaveState has no RequiresDynamicCode attribute.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task SaveStateWithTypeInfo_DoesNotHaveRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethods()
            .FirstOrDefault(m => m.Name == nameof(WinRTAppDataDriver.SaveState) && m.GetParameters().Length == 2);

        var attribute = method?
            .GetCustomAttributes(typeof(RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNull();
    }

    /// <summary>Validates that typed LoadState has no RequiresDynamicCode attribute.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task LoadStateWithTypeInfo_DoesNotHaveRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethods()
            .FirstOrDefault(m =>
                m.Name == nameof(WinRTAppDataDriver.LoadState)
                && m.GetParameters().Length == 1
                && m.IsGenericMethod);

        var attribute = method?
            .GetCustomAttributes(typeof(RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNull();
    }

    /// <summary>Validates that InvalidateState method does not have AOT-unsafe attributes.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_DoesNotHaveRequiresDynamicCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(nameof(WinRTAppDataDriver.InvalidateState));

        var attribute = method?
            .GetCustomAttributes(typeof(RequiresDynamicCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNull();
    }

    /// <summary>Validates that InvalidateState method does not have AOT-unsafe attributes.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task InvalidateState_DoesNotHaveRequiresUnreferencedCodeAttribute()
    {
        var method = typeof(WinRTAppDataDriver).GetMethod(nameof(WinRTAppDataDriver.InvalidateState));

        var attribute = method?
            .GetCustomAttributes(typeof(RequiresUnreferencedCodeAttribute), false)
            .FirstOrDefault();

        await Assert.That(attribute).IsNull();
    }
}

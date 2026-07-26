// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Storage;

/// <summary>Contains shared fixture setup for WinRT application data driver tests.</summary>
[NotInParallel("WinRTAppDataDriverStorage")]
public partial class WinRTAppDataDriverTests
{
    /// <summary>The default value used by test state instances.</summary>
    private const int DefaultStateValue = 42;

    /// <summary>The delay used to observe subscription callbacks.</summary>
    private const int SubscriptionDelayMilliseconds = 100;

    /// <summary>The delay used to observe observable execution callbacks.</summary>
    private const int ExecutionDelayMilliseconds = 200;

    /// <summary>The expected minimum overload count for reflected method checks.</summary>
    private const int ExpectedOverloadCount = 2;

    /// <summary>The value used by nested inner test state.</summary>
    private const int NestedInnerValue = 99;

    /// <summary>The value used by JSON round-trip state.</summary>
    private const int JsonRoundTripValue = 84;

    /// <summary>The value used by invalidation state.</summary>
    private const int InvalidationStateValue = 126;

    /// <summary>The long string length used by serialization shape tests.</summary>
    private const int LongStringLength = 10_000;

    /// <summary>The negative value used by serialization shape tests.</summary>
    private const int NegativeStateValue = -42;

    /// <summary>The value used by deeply nested test state.</summary>
    private const int DeepNestedValue = 100;

    /// <summary>The second value used by distinct state instance tests.</summary>
    private const int SecondStateValue = 2;

    /// <summary>The XML payload used by XML parsing tests.</summary>
    private const string XmlStatePayload = "<state />";

    /// <summary>Stores the suspension driver under test.</summary>
    private WinRTAppDataDriver _sut = null!;

    /// <summary>Sets up the test by creating a new instance of WinRTAppDataDriver.</summary>
    [Before(Test)]
    public void SetUp() => _sut = new();

    /// <summary>Validates that WinRTAppDataDriver implements ISuspensionDriver interface.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WinRTAppDataDriver_ImplementsISuspensionDriver() =>
        await Assert.That(_sut).IsAssignableTo<ISuspensionDriver>();

    /// <summary>Validates that multiple instances can be created independently.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Constructor_AllowsMultipleInstances()
    {
        var sut1 = new WinRTAppDataDriver();
        var sut2 = new WinRTAppDataDriver();

        await Assert.That(sut1).IsNotNull();
        await Assert.That(sut2).IsNotNull();
        await Assert.That(sut1).IsNotSameReferenceAs(sut2);
    }

    /// <summary>Validates that default constructor creates a valid instance.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task Constructor_CreatesValidInstance()
    {
        var driver = new WinRTAppDataDriver();

        await Assert.That(driver).IsNotNull();
        await Assert.That(driver).IsAssignableTo<WinRTAppDataDriver>();
    }

    /// <summary>Validates that the class can be used in a using statement pattern.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task WinRTAppDataDriver_CanBeUsedInScopePattern()
    {
        var driver = new WinRTAppDataDriver();

        await Assert.That(driver).IsNotNull();

        // Driver should still be usable outside the scope
        await Assert.That(driver).IsNotNull();
        await Assert.That(driver.InvalidateState()).IsNotNull();
    }

    /// <summary>Determines whether an exception indicates unavailable application storage.</summary>
    /// <param name="exception">The exception thrown while accessing application storage.</param>
    /// <returns><c>true</c> if application storage is unavailable; otherwise, <c>false</c>.</returns>
    private static bool IsApplicationStorageUnavailable(Exception exception) =>
        exception is InvalidOperationException or NotSupportedException or UnauthorizedAccessException;

    /// <summary>Skips a test because application storage is unavailable.</summary>
    /// <param name="exception">The exception thrown while accessing application storage.</param>
    private static void SkipUnavailableApplicationStorage(Exception exception) =>
        Skip.Test($"Application storage is not available in this environment: {exception.Message}");
}

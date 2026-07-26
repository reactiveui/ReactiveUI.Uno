// Copyright (c) 2019-2026 ReactiveUI Association Incorporated. All rights reserved.
// ReactiveUI Association Incorporated licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TUnit.Assertions.Extensions;
using TUnit.Core;

namespace ReactiveUI.Uno.Tests.Observable;

/// <summary>Contains tests for dependency property observable creation.</summary>
public class DependencyObjectObservableForPropertyTests
{
    /// <summary>The expected affinity for dependency object properties.</summary>
    private const int ExpectedDependencyObjectAffinity = 6;

    /// <summary>The system under test.</summary>
    private DependencyObjectObservableForProperty _sut = null!;

    /// <summary>Sets up the test by creating a new instance of DependencyObjectObservableForProperty.</summary>
    [Before(Test)]
    public void SetUp() => _sut = new();

    /// <summary>Validates DependencyObjectObservableForProperty interface implementation.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task DependencyObjectObservableForProperty_ImplementsICreatesObservableForProperty() =>
        await Assert.That(_sut).IsAssignableTo<ICreatesObservableForProperty>();

    /// <summary>Validates that GetAffinityForObject returns zero for non-DependencyObject types.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsZero_ForNonDependencyObjectTypes()
    {
        var affinity = _sut.GetAffinityForObject(typeof(object), "TestProperty");
        await Assert.That(affinity).IsZero();
    }

    /// <summary>Validates that GetAffinityForObject returns zero for string types.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsZero_ForStringType()
    {
        var affinity = _sut.GetAffinityForObject(typeof(string), "Length");
        await Assert.That(affinity).IsZero();
    }

    /// <summary>Validates that GetAffinityForObject returns zero for value types.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsZero_ForValueTypes()
    {
        var affinity = _sut.GetAffinityForObject(typeof(int), "MaxValue");
        await Assert.That(affinity).IsZero();
    }

    /// <summary>Validates that GetAffinityForObject returns zero for DependencyObject with invalid property.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsZero_ForDependencyObjectWithInvalidProperty()
    {
        var affinity = _sut.GetAffinityForObject(typeof(FrameworkElement), "NonExistentProperty");
        await Assert.That(affinity).IsZero();
    }

    /// <summary>Validates affinity for a valid framework element property.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsPositiveAffinity_ForFrameworkElementWithValidProperty()
    {
        var affinity = _sut.GetAffinityForObject(typeof(FrameworkElement), nameof(FrameworkElement.Width));
        await Assert.That(affinity).IsEqualTo(ExpectedDependencyObjectAffinity);
    }

    /// <summary>Validates affinity for the content control content property.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsPositiveAffinity_ForContentControlWithContentProperty()
    {
        var affinity = _sut.GetAffinityForObject(typeof(ContentControl), nameof(ContentControl.Content));
        await Assert.That(affinity).IsEqualTo(ExpectedDependencyObjectAffinity);
    }

    /// <summary>
    /// Validates that GetAffinityForObject returns positive affinity even for before changed scenarios.
    /// Note: The implementation returns affinity regardless of beforeChanged flag, but GetNotificationForProperty
    /// falls back to POCO for beforeChanged scenarios.
    /// </summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsPositiveAffinity_ForBeforeChanged()
    {
        var affinity = _sut.GetAffinityForObject(
            typeof(FrameworkElement),
            nameof(FrameworkElement.Width),
            beforeChanged: true);
        await Assert.That(affinity).IsEqualTo(ExpectedDependencyObjectAffinity);
    }

    /// <summary>Validates that GetNotificationForProperty throws ArgumentNullException when sender is null.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetNotificationForProperty_ThrowsArgumentNullException_WhenSenderIsNull()
    {
        Expression<Func<object>> expr = () => new object();
        var exception = await Assert.That(
            () => _sut.GetNotificationForProperty(null!, expr, "Property")).Throws<ArgumentNullException>();
        await Assert.That(exception!.ParamName).IsEqualTo("sender");
    }

    /// <summary>Validates that a non-dependency-object sender throws <see cref="ArgumentException"/>.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetNotificationForProperty_ThrowsArgumentException_WhenSenderIsNotDependencyObject()
    {
        var sender = new object();
        Expression<Func<object>> expr = () => sender;
        var exception = await Assert.That(
            () => _sut.GetNotificationForProperty(sender, expr, "Property")).Throws<ArgumentException>();
        await Assert.That(exception!.ParamName).IsEqualTo("sender");
    }

    /// <summary>Validates that multiple instances can be created independently.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task MultipleInstances_CanBeCreatedIndependently()
    {
        var sut1 = new DependencyObjectObservableForProperty();
        var sut2 = new DependencyObjectObservableForProperty();

        await Assert.That(sut1).IsNotNull();
        await Assert.That(sut2).IsNotNull();
        await Assert.That(sut1).IsNotSameReferenceAs(sut2);
    }

    /// <summary>Validates that repeated affinity queries return consistent results.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsConsistentResults_ForSameTypeAndProperty()
    {
        var affinity1 = _sut.GetAffinityForObject(typeof(FrameworkElement), nameof(FrameworkElement.Width));
        var affinity2 = _sut.GetAffinityForObject(typeof(FrameworkElement), nameof(FrameworkElement.Width));

        await Assert.That(affinity1).IsEqualTo(affinity2);
    }

    /// <summary>Validates that GetAffinityForObject returns zero for interface types.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsZero_ForInterfaceTypes()
    {
        var affinity = _sut.GetAffinityForObject(typeof(IDisposable), "TestProperty");
        await Assert.That(affinity).IsZero();
    }

    /// <summary>Validates affinity for a derived dependency object type.</summary>
    /// <returns>A task that represents the asynchronous test.</returns>
    [Test]
    public async Task GetAffinityForObject_ReturnsPositiveAffinity_ForDerivedDependencyObjectTypes()
    {
        var affinity = _sut.GetAffinityForObject(typeof(Button), nameof(Button.Content));
        await Assert.That(affinity).IsEqualTo(ExpectedDependencyObjectAffinity);
    }
}

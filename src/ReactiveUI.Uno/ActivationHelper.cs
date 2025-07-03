// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

namespace ReactiveUI.Uno;

internal static class ActivationHelper
{
    static ActivationHelper()
    {
        if (UnoActivated)
        {
            return;
        }

        UnoActivated = true;
        Locator.RegisterResolverCallbackChanged(() =>
        {
            if (Locator.CurrentMutable is null)
            {
                return;
            }

            new Registrations().Register((f, t) => Locator.CurrentMutable.RegisterConstant(f(), t));
        });
    }

    internal static bool UnoActivated { get; }
}

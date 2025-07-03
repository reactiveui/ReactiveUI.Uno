// Copyright (c) 2025 ReactiveUI and Contributors. All rights reserved.
// Licensed to reactiveui and contributors under one or more agreements.
// The reactiveui and contributors licenses this file to you under the MIT license.
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

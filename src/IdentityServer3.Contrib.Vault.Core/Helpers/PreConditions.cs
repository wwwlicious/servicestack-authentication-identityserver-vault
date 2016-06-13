// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.Core.Helpers
{
    using System;

    public static class PreConditions
    {
        /// <summary>Throw ArgumentNullException if arg is null</summary>
        /// <typeparam name="T">Type of Arg</typeparam>
        /// <param name="arg">arg</param>
        /// <param name="name">Name of the argument</param>
        /// <returns>arg</returns>
        public static T ThrowIfNull<T>(this T arg, string name)
        {
            if (arg == null) throw new ArgumentNullException(name);

            return arg;
        }

        /// <summary>Throw ArgumentNullException if arg is null</summary>
        /// <param name="arg">arg</param>
        /// <param name="name">Name of the argument</param>
        /// <returns>arg</returns>
        public static string ThrowIfNullOrEmpty(this string arg, string name)
        {
            if (string.IsNullOrWhiteSpace(arg)) throw new ArgumentNullException(name);

            return arg;
        }
    }
}

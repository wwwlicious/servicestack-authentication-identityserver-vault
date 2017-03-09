// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.CertificateStore.Exceptions
{
    using System;

    public class FailedOperationException : Exception
    {
        public FailedOperationException(string message) : base(message)
        {            
        }
    }
}

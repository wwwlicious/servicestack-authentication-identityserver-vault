﻿// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.ServiceStack.Vault.CertificateStore.Demo.ServiceInterface
{
    using global::ServiceStack;
    using global::ServiceStack.Authentication.IdentityServer.Providers;
    using ServiceModel;

    [Authenticate(IdentityServerAuthProvider.Name)]
    public class HelloServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Many bothans died to bring you this information." };
        }
    }
}
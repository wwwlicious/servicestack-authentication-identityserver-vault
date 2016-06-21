// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.Core
{
    using System;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using Interfaces;

    public class VaultClientUri : IVaultClientUri
    {
        public VaultClientUri(string vaultUri, X509Certificate2 certificate)
        {
            vaultUri.ThrowIfNullOrEmpty(nameof(vaultUri));

            VaultUri = vaultUri;

            ServiceClientFunc = () =>
            {
                var client = new JsonServiceClient(VaultUri);
                if (certificate != null)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    client.RequestFilter += request =>
                    {
                        request.ClientCertificates.Add(certificate);
                    };
                }

                return client;
            };
        }

        public Func<IJsonServiceClient> ServiceClientFunc { get; set; } 

        public string VaultUri { get; }

        public IJsonServiceClient ServiceClient => ServiceClientFunc();
    }
}

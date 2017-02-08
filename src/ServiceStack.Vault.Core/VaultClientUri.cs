namespace ServiceStack.Vault.Core
{
    using System;
    using System.Net.Http;
    using System.Security.Authentication;
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
                var client = new JsonHttpClient(vaultUri);
                if (certificate != null)
                {
#if NETSTANDARD1_6
                    var handler = new HttpClientHandler
                    {
                        ClientCertificateOptions = ClientCertificateOption.Automatic
                    };
                    handler.SslProtocols = SslProtocols.Tls12;
#elif NET45
                    var handler = new WebRequestHandler();                       
#endif
                    handler.ClientCertificates.Add(certificate);
                    client.HttpMessageHandler = handler;
                }
                return client;
            };
        }

        public Func<IJsonServiceClient> ServiceClientFunc { get; set; }

        public string VaultUri { get; }

        public IJsonServiceClient ServiceClient => ServiceClientFunc();
    }
}

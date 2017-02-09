// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer4.Contrib.Vault.Core
{
    using System;
    using System.Threading.Tasks;

    using System.Net.Http;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using Interfaces;
    using Newtonsoft.Json;

    class VaultHttpClient : IVaultHttpClient
    {
        private readonly HttpClient httpClient;

        public VaultHttpClient(string vaultUri, X509Certificate2 cert)
        {
            if (vaultUri.StartsWith("https"))
            {
                if (cert == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(vaultUri), "https requires an X509Certificate");
                }

                var handler = new HttpClientHandler
                {
                    ClientCertificateOptions = ClientCertificateOption.Automatic,
                    SslProtocols = SslProtocols.Tls12
                };
                handler.ClientCertificates.Add(cert);

                httpClient = new HttpClient(handler) { BaseAddress = new Uri(vaultUri) };
            }
            else
            {
                httpClient = new HttpClient { BaseAddress = new Uri(vaultUri) };
            }
        }

        public void AddHeader(string name, string value)
        {
            httpClient.DefaultRequestHeaders.Add(name, new[] { value });
        }

        public TResponse Post<TRequest, TResponse>(string endpoint, TRequest request)
        {
            return PostAsync<TRequest, TResponse>(endpoint, request).Result;
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endPoint, TRequest request)
        {
            var requestStr = JsonConvert.SerializeObject(request);

            var response = await httpClient.PostAsync(endPoint, new StringContent(requestStr))
                                           .ConfigureAwait(false);

            var responseStr = await response.Content.ReadAsStringAsync()
                                                    .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(responseStr))
            {
                return default(TResponse);
            }
            return JsonConvert.DeserializeObject<TResponse>(responseStr);
        }

        public async Task<TResponse> GetAsync<TResponse>(string endPoint)
        {
            var response = await httpClient.GetAsync(endPoint).ConfigureAwait(false);

            var responseStr = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(responseStr))
            {
                return default(TResponse);
            }
            return JsonConvert.DeserializeObject<TResponse>(responseStr);
        }

        public void Dispose()
        {
            httpClient?.Dispose();
        }
    }
}

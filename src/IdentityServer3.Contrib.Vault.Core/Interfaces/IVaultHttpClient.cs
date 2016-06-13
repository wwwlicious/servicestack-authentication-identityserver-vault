// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.Core.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface IVaultHttpClient : IDisposable
    {
        void AddHeader(string name, string value);

        TResponse Post<TRequest, TResponse>(string endPoint, TRequest request);

        Task<TResponse> PostAsync<TRequest, TResponse>(string endPoint, TRequest request);

        Task<TResponse> GetAsync<TResponse>(string endPoint);
    }
}

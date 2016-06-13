// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Owin;

    public interface IRequestParser
    {
        IHeaderDictionary HeaderDictionary(IDictionary<string, object> environment);

        Task<IFormCollection> ReadRequestFormAsync(IDictionary<string, object> environment);
    }
}

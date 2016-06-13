// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Helpers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.IO;
    using Interfaces;
    using Microsoft.Owin;

    public class RequestParser : IRequestParser
    {
        public IHeaderDictionary HeaderDictionary(IOwinContext context)
        {
            context.ThrowIfNull(nameof(context));         

            return context.Request.Headers;
        }

        public async Task<IFormCollection> ReadRequestFormAsync(IOwinContext context)
        {
            context.ThrowIfNull(nameof(context));

            // hack to clear a possible cached type from Katana in environment
            context.Environment.Remove("Microsoft.Owin.Form#collection");

            if (!context.Request.Body.CanSeek)
            {
                var copy = new MemoryStream();
                await context.Request.Body.CopyToAsync(copy).ConfigureAwait(false);
                copy.Seek(0L, SeekOrigin.Begin);
                context.Request.Body = copy;
            }

            context.Request.Body.Seek(0L, SeekOrigin.Begin);
            var form = await context.Request.ReadFormAsync().ConfigureAwait(false);
            context.Request.Body.Seek(0L, SeekOrigin.Begin);

            // hack to prevent caching of an internalized type from Katana in environment
            context.Environment.Remove("Microsoft.Owin.Form#collection");

            return form;
        }

        public IHeaderDictionary HeaderDictionary(IDictionary<string, object> environment)
        {
            return HeaderDictionary(new OwinContext(environment));
        }

        public Task<IFormCollection> ReadRequestFormAsync(IDictionary<string, object> environment)
        {
            return ReadRequestFormAsync(new OwinContext(environment));
        }
    }
}

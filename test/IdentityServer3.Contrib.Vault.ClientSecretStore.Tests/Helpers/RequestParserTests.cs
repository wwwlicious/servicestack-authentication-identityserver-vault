// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace IdentityServer3.Contrib.Vault.ClientSecretStore.Tests.Helpers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using FluentAssertions;
    using Microsoft.Owin;
    using Vault.ClientSecretStore.Helpers;
    using Xunit;

    public class RequestParserTests
    {
        [Fact]
        public void HeaderDictionary_GetHeaders_Success()
        {
            // Arrange
            var context = new OwinContext();
            context.Request.Headers.Add(new KeyValuePair<string, string[]>("Authorization", new []{ "Basic 12345:6789" }));
            
            var parser = new RequestParser();

            // Act
            var result = parser.HeaderDictionary(context.Environment);

            // Assert
            result.Count.Should().Be(1);
            result["Authorization"].Should().Be("Basic 12345:6789");
        }

        [Fact]
        public void ReadRequestFormAsync_Success()
        {
            // Arrange
            var context = new OwinContext();
            var body = "client_id=client&client_secret=secret";
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));

            var parser = new RequestParser();

            // Act
            var form = parser.ReadRequestFormAsync(context.Environment).Result;

            // Assert
            form.Count().Should().Be(2);
            form["client_id"].Should().Be("client");
            form["client_secret"].Should().Be("secret");
        }
    }
}

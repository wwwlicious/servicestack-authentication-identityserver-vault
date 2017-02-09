// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.SecretStore.ClientDemo
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using Configuration;
    using Microsoft.IdentityModel.Protocols;
    using Text;
    using System.Configuration;

    class Program
    {
        static void Main(string[] args)
        {
            var serviceName = ConfigurationManager.AppSettings["ServiceName"];

            var appSettings = new AppSettings();
            
            // Now start up service stack client
            new AppHost("http://localhost:5001/", appSettings).Init().Start("http://*:5001/");
            "ServiceStack Self Host with Razor listening at http://localhost:5001 ".Print();
            Process.Start("http://localhost:5001/");

            Console.ReadLine();
        }
    }
}

// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at http://mozilla.org/MPL/2.0/.
namespace ServiceStack.Vault.SecretAppSettings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Configuration;
    using Core.Helpers;
    using Core.Interfaces;
    using DTO;
    using Logging;
    using Text;

    public class VaultSecretAppSettings : IAppSettings
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(VaultSecretAppSettings));

        private readonly IVaultClient vaultClient;

        private readonly string secretsPath;

        /// <summary>Constructor</summary>
        /// <param name="vaultClient">Vault Client</param>
        /// <param name="secretsPath">Secrets Path</param>
        public VaultSecretAppSettings(IVaultClient vaultClient, string secretsPath)
        {
            vaultClient.ThrowIfNull(nameof(vaultClient));
            secretsPath.ThrowIfNullOrEmpty(nameof(secretsPath));

            this.vaultClient = vaultClient;
            this.secretsPath = secretsPath;
        }

        /// <summary>Get all keys</summary>
        /// <returns></returns>
        public Dictionary<string,string> GetAll()
        {
            var keys = GetAllKeys();
            var all = new Dictionary<string, string>();
            foreach (var key in keys)
            {
                var value = GetString(key);
                if (value != null)
                {
                    all.Add(key, value);
                }
            }
            return all;
        }

        /// <summary>Gets all keys from the Defined Path</summary>
        /// <returns>List of Keys</returns>
        public List<string> GetAllKeys()
        {     
            var keys = new List<string>();
            string path = secretsPath;
            if (!path.EndsWith("/"))
            {
                path = path + "/";
            }              
            GetKeys("", path, keys);
            return keys;
        }

        /// <summary>Handles traversing vault keys</summary>
        /// <param name="prefix">Current branch prefix</param>
        /// <param name="path">Path</param>
        /// <param name="keys">List of Keys already found</param>
        private void GetKeys(string prefix, string path, List<string> keys)
        {
            List<string> listKeys;
            using (var client = this.vaultClient.ServiceClient)
            {
                var response = client.Get(new ListSecrets { Path = path });
                if (response.Data?.Keys == null || response.Data.Keys.Length == 0)
                {
                    return;
                }
                listKeys = response.Data.Keys.ToList();
            }

            foreach (var key in listKeys)
            {
                if (key.EndsWith("/"))
                {
                    GetKeys(key, path + key, keys);
                }
                else
                {
                    keys.Add(prefix + key);
                }
            }
        } 

        /// <summary>Returns true if the Key exists</summary>
        /// <param name="key">Key</param>
        /// <returns>True if Key Exists</returns>
        public bool Exists(string key)
        {
            var result = Get<string>(key, null);
            return result != null;
        }

        /// <summary>Sets a Key Value</summary>
        /// <typeparam name="T">Type of Value</typeparam>
        /// <param name="key">Key (path to Key)</param>
        /// <param name="value">Value</param>
        public void Set<T>(string key, T value)
        {
            using (var client = this.vaultClient.ServiceClient)
            {
                client.Put(new WriteSecret
                {
                    Key = $"{secretsPath}/{key}",
                    Value = Encoding.UTF8.GetBytes(TypeSerializer.SerializeToString(value))
                });
            }            
        }

        /// <summary>Gets the Key as a String</summary>
        /// <param name="name">Name of the Key</param>
        /// <returns>Key Value</returns>
        public string GetString(string name)
        {
            return Get<string>(name, null);
        }

        /// <summary>Gets the Key as a list of Strings</summary>
        /// <param name="key">Name of the Key</param>
        /// <returns>List of Values</returns>
        public IList<string> GetList(string key)
        {
            return Get<List<string>>(key, null);
        }

        public IDictionary<string, string> GetDictionary(string key)
        {
            return Get<Dictionary<string, string>>(key, null);
        }

        public T Get<T>(string name)
        {
            return Get(name, default(T));
        }

        public T Get<T>(string name, T defaultValue)
        {
            name.ThrowIfNullOrWhitespace(nameof(name));

            try
            {
                using (var client = this.vaultClient.ServiceClient)
                {
                    var response = client.Get(new ReadSecrets
                    {
                        Key = $"{secretsPath}/{name}"
                    });
                    return response.GetValue<T>();
                }
            }
            catch (WebException ex) when (ex.ToStatusCode() == 404)
            {
                Log.Error($"Unable to find secret value with key {name}", ex);
                return defaultValue;
            }
            catch (NotSupportedException ex)
            {
                Log.Error($"Unable to deserialise secret value with key {name}", ex);
                return defaultValue;
            }
            catch (Exception ex)
            {
                Log.Error($"Error getting string value for secret key {name}", ex);
                return defaultValue;
            }
        }
    }
}

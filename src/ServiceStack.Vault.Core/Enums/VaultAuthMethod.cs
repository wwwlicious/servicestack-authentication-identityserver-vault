namespace ServiceStack.Vault.Core.Enums
{
    using System;

    public enum VaultAuthMethod
    {
        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        AppId = 0,

        AppRole = 1,

        User = 2
    }
}

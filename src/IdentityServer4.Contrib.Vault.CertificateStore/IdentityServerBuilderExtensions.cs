namespace IdentityServer4.Contrib.Vault.CertificateStore
{
    using System;
    using Core;
    using Core.Interfaces;
    using Core.VaultAuth;
    using Helpers;
    using Interfaces;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Options;
    using Stores;

    public static class IdentityServerBuilderExtensions
    {
        [Obsolete("AppId Auth Backend has been deprecated from Vault as of Version version 0.6.1")]
        public static IIdentityServerBuilder AddVaultAppIdCertificateStore(this IIdentityServerBuilder builder, VaultCertificateStoreAppIdOptions options)
        {
            builder.Services.AddSingleton<IVaultAuth>(resolver => 
                new VaultAppIdAuth(options.AppId, options.UserId, resolver.GetService<ILogger<VaultAppIdAuth>>())
            );
            builder.AddVaultCertificateStore(options);
            return builder;
        }

        public static IIdentityServerBuilder AddVaultAppRoleCertificateStore(this IIdentityServerBuilder builder, VaultCertificateStoreAppRoleOptions options)
        {
            builder.Services.AddSingleton<IVaultAuth>(resolver => 
                new VaultAppRoleAuth(options.RoleId, options.SecretId, resolver.GetService<ILogger<VaultAppRoleAuth>>())
            );
            builder.AddVaultCertificateStore(options);
            return builder;
        }

        public static IIdentityServerBuilder AddVaultCertificateStore(this IIdentityServerBuilder builder, VaultCertificateStoreOptions vaultOptions)
        {
            builder.Services.AddSingleton(vaultOptions);

            builder.Services.AddSingleton<IVaultClient>(resolver => new VaultClient(resolver.GetService<IVaultAuth>(), vaultOptions.VaultUrl, vaultOptions.VaultCertificate));

            builder.Services.AddSingleton<IVaultCertificateStore>(resolver => 
                new VaultCertificateStore(resolver.GetService<IVaultClient>(), vaultOptions.RoleName, vaultOptions.CommonName, resolver.GetService<ILogger<VaultCertificateStore>>())
            );
            builder.Services.AddSingleton<IX509Certificate2Helper, X509Certificate2Helper>();
            builder.Services.AddSingleton<IRSACryptoServiceProviderHelper, RsaCryptoServiceProviderHelper>();

            builder.Services.AddSingleton<IVaultCertificateService, VaultCertificateService>();

            builder.Services.AddSingleton<ISigningCredentialStore, VaultSigningCredentialStore>();
            builder.Services.AddSingleton<IValidationKeysStore, VaultValidationKeysStore>();

            return builder;
        }
    }
}

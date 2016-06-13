# ServiceStack.Auth.Vault

[Vault](https://www.vaultproject.io/) is a tool for managing secrets that provides a restful api.

This solution is divided 2 pieces of functionality:
## Vault as a ClientSecretStore
The IdentityServerVaultAuthFeature is an extension of the [IdentityServerAuthFeature](https://github.com/MacLeanElectrical/servicestack-authentication-identityserver) that retrieves the Client Secret for a Client ID, encrypts that secret using Vault as part of the IdentityServerAuthProvider Authorization.
The IdentityServer3.Contrib.Vault.ClientSecretStore is the corresponding IdentityServer3 plugin for decrypting the secret received from the ServiceStack IdentityServerAuthProvider.

Read the Quick Start guide for using vault as the client secret store [here](docs/vault_clientsecretstore.md)

Read the sample guide [here](docs/vault_clientsecretstore_sample.md) or view the code [here](samples/IdentityServer3.Contrib/IdentityServer3.Contrib.Vault.ClientSecretStore.Demo)

## Vault as a CertificateStore
The IdentityServer3.Contrib.Vault.CertificateStore is a plugin that allows IdentityServer to generate new X509 Signing Certificates and replace expired certificates using Vault.

Read the Quick Start guid for using vault as the X509 Certificate store [here](docs/vault_certificatestore.md)

Read the sample guide [here](docs/vault_certificatestore_sample.md) or view the code [here](samples/IdentityServer3.Contrib/IdentityServer3.Contrib.Vault.CertificateStore.Demo)
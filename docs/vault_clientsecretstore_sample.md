# IdentityServer3.Contrib.Vault.ClientSecretStore.Demo

A demo project that authenticates a [ServiceStack](https://servicestack.net/) razor-based Client App using [IdentityServer](https://identityserver.github.io/) and [Vault](https://www.vaultproject.io/)  as the Client Secret Store

## Overview
This demo project bring in the various Identity Server and Service Stack plugins available in this Solution, namely:
* IdentityServer3.Contrib.ServiceStack - An IdentityServer plugin that supports impersonation authentication of a ServiceStack instance using IdentityServer
* IdentityServer3.Contrib.Vault.ClientSecretStore - An IdentityServer plugin that uses Vault to store and authenticate client secrets
* ServiceStack.Authentication.IdentityServer.Vault - A ServiceStack AuthProvider that authenticates a user against an IdentityServer instance and uses Vault to store and authenticate client secrets

When the project starts, you should be presented with a simple ServiceStack web app with a link that redirects to a secure service in ServiceStack. When you select the link you should be redirected to the IdentityServer instance that prompts you for login details.  Login using username "test@test.com" with password "password123".  You should then be redirected back to the ServiceStack web app and have access to the secure service (with Authenticate attribute) which displays the secure message.

### Prerequisites
* Have an unitialised instance of Vault running locally on port 8200.  See below for instructions.
* Run the Setup_Samples.ps1 powershell script, located in /samples/Setup (must be run from the directory it is located in).

#### Starting an instance of vault
Having downloaded and extracted the Vault.exe, start an unitialised instance of Vault, run the following vault command:
```bat
vault.exe server -conf=vault.conf
```

Where vault.conf contains the following configuration:
```hcl
listener "tcp" {
    address = "127.0.0.1:8200"
    tls_disable = 1
}
```
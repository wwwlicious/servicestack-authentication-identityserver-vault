# Starting an instance of vault
Having downloaded and extracted the Vault.exe, start an unitialised instance of Vault, run the following vault command:
    vault.exe server -config=vault.conf
        
Where vault.conf contains the following configuration (consul backend) and assuming consul is already running:
```hcl
backend "consul" {
	tls_skip_verify = 1
}
    
listener "tcp" {
    address = "127.0.0.1:8200"
    tls_disable = 1
}
```

If consul is not already running, open up a command prompt (assuming consul location is set in the environment path):
```sh
$ consul agent -dev
```

## Initialize Vault
Open up another command prompt and set the Vault address variable e.g. (for windows)
```sh
$ set VAULT_ADDR=http://localhost:8200/   
```

Then run the following command (assuming vault location is set in the environment path):
```sh
$ vault init
```

The response should contain a set of keys and a root token - these are important so copy and paste these into an editor. e.g.
```sh
Key 1: cc308a21739c69aaac628eaf6936ffd870ddd310e8575865884584a6d652bfcb01
Key 2: 8e5549969871b4abcfbf7cd34b71625f1cf5969a20db44d7fb645f1ab351895f02
Key 3: be03c806dbdab44a74ee3fa8cce39c0783580bd0fc9a7c7f57a970bc11f00e0603
Key 4: 504af88fc39791745e8868334990e182675c2c8af6b063a6c7e4c61bb76de7f404
Key 5: 601c791f803c9195e5d92b48ce021fdaf8f1b1c02af15b0e6b29e9bd15cc60ad05
Initial Root Token: 232042b2-fe94-363d-2725-15ea1a6f3094

Vault initialized with 5 keys and a key threshold of 3. Please
securely distribute the above keys. When the Vault is re-sealed,
restarted, or stopped, you must provide at least 3 of these keys
to unseal it again.

Vault does not store the master key. Without at least 3 keys,
your Vault will remain permanently sealed.
```

Set the Initial Root Token as the vault token environmental variable to perform Admin operations in vault. e.g. (for windows)
```sh
$ set VAULT_TOKEN=232042b2-fe94-363d-2725-15ea1a6f3094
```

## Unseal Vault
Run the following command for the number of keys required to reach the threshold. So for the example above, 3 keys are required. e.g.
```sh
$ vault unseal
key (will be hidden): cc308a21739c69aaac628eaf6936ffd870ddd310e8575865884584a6d652bfcb01    # Key shown for example
Key Shares: 5
Key Threshold: 3
Unseal Progress: 1
```
Once the threshold of keys has been reached, vault will unseal.


## AppID Auth Backend
Run the following command to mount the AppId Auth backend:
```sh
$ vault auth-enable app-id
Successfully enabled 'app-id' at 'app-id'!
```
To create an app-id / user-id combination foo, bar that will give root access:
```sh
$ vault write auth/app-id/map/app-id/foo value=root display_name=foo
$ vault write auth/app-id/map/user-id/bar value=foo
```
See more on the App ID Auth Backend [here](https://www.vaultproject.io/docs/auth/app-id.html)

## Transit Secret Backend
Run the following command to mount the transit backend:
```sh
$ vault mount transit
Successfully mounted 'transit' at 'transit'!
```
To create an encryption key for a ServiceStack Client with Client Id "SERVICE1", run the following command:
```sh
    $ vault write -f transit/keys/SERVICE1
    Success! Data written to: transit/keys/SERVICE1
```
See more on the Transit Secret Backend [here](https://www.vaultproject.io/docs/secrets/transit/index.html)

## Certificate Generation Backend
To configure Vault to generate tokens based on a generated root token with common name = "test.com" and role name ="example.test.com"
```sh
    $ vault mount pki
    $ vault mount-tune -max-lease-ttl=87600h pki
    $ vault write pki/root/generate/internal common_name=test.com ttl=87600h
    $ vault write pki/config/urls issuing_certificates="http://127.0.0.1:8200/v1/pki/ca" crl_distribution_points="http://127.0.0.1:8200/v1/pki/crl"
    $ vault vault write pki/roles/example.test.com allowed_domains="test.com" allow_subdomains="true" max_ttl="72h"    
```
See more on the Certificate Generation Backend [here](https://www.vaultproject.io/docs/secrets/pki/index.html)
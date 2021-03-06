function Initialise
{
    Param(
        [Parameter(Mandatory=$true)] [string] $vault_url, 
        [int] $secret_shares=5, 
        [int] $secret_threshold=3
    )

    Write-Host "Initialising Vault using secret_shares: $secret_shares, secret_threshold: $secret_threshold"
    return Invoke-RestMethod -Uri "$vault_url/v1/sys/init" -Method "Put" -Body "{`"secret_shares`":$secret_shares, `"secret_threshold`":$secret_threshold}"
}

function Unseal
{
    Param(
        [Parameter(Mandatory=$true)] [string] $vault_url, 
        [Parameter(Mandatory=$true)] [string] $key
    )

    Write-Host "Unsealing Vault using $key"
    Invoke-RestMethod -Uri "$vault_url/v1/sys/unseal" -Method "Put" -Body "{`"key`":`"$key`"}"
}

function RootTokenHeader
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token
    )

    $root_tokenHeader = @{}
    $root_tokenHeader.Add("X-Vault-Token", $root_token)
    return $root_tokenHeader
}

function CreateSecrets
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token, 
        [Parameter(Mandatory=$true)] [string] $vault_url, 
        [Parameter(Mandatory=$true)] [string] $secret_path, 
        [Parameter(Mandatory=$true)] [string[]] $secrets
    )

    Write-Host "Create Secrets at /v1/secret/$secret_path"

    $secrets_body = @{}

    for($i=0; $i -lt $secrets.Length; $i++)
    {
        $key = "$i"
        $secret = $secrets[$i]

        $secrets_body.Add($key, $secret);
    }
    $secrets_json = ConvertTo-Json $secrets_body

    $root_tokenHeader = RootTokenHeader -root_token $root_token
    Invoke-RestMethod -Headers $root_tokenHeader -Uri "$vault_url/v1/secret/$secret_path" -Method "Post" -Body $secrets_json
}

function Policy
{
    Param(
        [Parameter(Mandatory=$true)] [string] $path, 
        [Parameter(Mandatory=$true)] [string[]] $capabilities
    )

    $capabilities_json = ConvertTo-Json -Compress $capabilities
    return "path `"$path`" { capabilities=$capabilities_json }"
}

function CreatePolicy
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token, 
        [Parameter(Mandatory=$true)] [string] $vault_url,
        [Parameter(Mandatory=$true)] [string] $policy_name,
        [Parameter(Mandatory=$true)] [string] $path,
        [Parameter(Mandatory=$true)] [string[]] $capabilities 
    )

    $rules = Policy -path $path -capabilities $capabilities
    $policy = @{
        rules = $rules
    }
    $policy_json = ConvertTo-Json -Compress $policy

    $root_tokenHeader = RootTokenHeader -root_token $root_token

    Write-Host "Creating policy $policy_name with rules $rules"
    Invoke-RestMethod -Headers $root_tokenHeader -Uri "$vault_url/v1/sys/policy/$policy_name" -Method "Put" -Body $policy_json
}

function MountTransitBackend
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token, 
        [Parameter(Mandatory=$true)] [string] $vault_url
    )

    Write-Host "Mount Transit Backend"
    $root_tokenHeader = RootTokenHeader -root_token $root_token
    Invoke-RestMethod -Headers $root_tokenHeader -Uri "$vault_url/v1/sys/mounts/transit" -Method "Post" -Body "{`"type`":`"transit`"}"
}

function CreateTransitKey
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token, 
        [Parameter(Mandatory=$true)] [string] $vault_url,
        [Parameter(Mandatory=$true)] [string] $key
    )

    Write-Host "Creating Transit Key $key"
    $root_tokenHeader = RootTokenHeader -root_token $root_token
    Invoke-RestMethod -Headers $root_tokenHeader -Uri "$vault_url/v1/transit/keys/$key" -Method "Post"
}

function MountAppRoleBackend
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token, 
        [Parameter(Mandatory=$true)] [string] $vault_url
    )

    Write-Host "Mount App Role Backend"
    $root_tokenHeader = RootTokenHeader -root_token $root_token
    Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/sys/auth/approle" -Method "Post" -Body "{`"type`":`"approle`"}"
}

function CreateAppRole
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token,
        [Parameter(Mandatory=$true)] [string] $vault_url,
        [Parameter(Mandatory=$true)] [string] $app_role,
        [Parameter(Mandatory=$true)] [string[]] $policies
    )

    $root_tokenHeader = RootTokenHeader -root_token $root_token
    $policies_body = @{
        policies = [String]::Join(",", $policies)
    }
    $policies_body_json = ConvertTo-Json $policies_body

    Write-Host "Creating AppRole $app_role with policies $policies_body_json"
    Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/auth/approle/role/$app_role" -Method "Post" -Body $policies_body_json
}

function CreateAppRoleId
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token,
        [Parameter(Mandatory=$true)] [string] $vault_url,
        [Parameter(Mandatory=$true)] [string] $app_role
    )

    Write-Host "Obtaining AppRoleId for $app_role"
    $root_tokenHeader = RootTokenHeader -root_token $root_token
    $response = Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/auth/approle/role/$app_role/role-id" -Method "Get"

    $app_role_response = ConvertFrom-Json $response
    return $app_role_response.data.role_id
}

function CreateAppRoleSecretId
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token,
        [Parameter(Mandatory=$true)] [string] $vault_url,
        [Parameter(Mandatory=$true)] [string] $app_role
    )

    $root_tokenHeader = RootTokenHeader -root_token $root_token
    $response = Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/auth/approle/role/$app_role/secret-id" -Method "Post"
    $app_role_response = ConvertFrom-Json $response
    return $app_role_response.data.secret_id
}

function MountPkiBackend
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token,
        [Parameter(Mandatory=$true)] [string] $vault_url
    )

    Write-Host "Mount Pki Backend"
    $root_tokenHeader = RootTokenHeader -root_token $root_token
    Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/sys/mounts/pki" -Method "Post" -Body "{`"type`":`"pki`"}"
}

function TunePkiBackend
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token,
        [Parameter(Mandatory=$true)] [string] $vault_url,
        [string] $max_lease_ttl = "87600h"
    )

    Write-Host "Tune Pki Backend with max_lease_ttl: $max_lease_ttl"
    $root_tokenHeader = RootTokenHeader -root_token $root_token

    Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/sys/mounts/pki/tune" -Method "Post" -Body "{`"max_lease_ttl`":`"$max_lease_ttl`"}"
}

function GeneratePkiRootCertificate
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token,
        [Parameter(Mandatory=$true)] [string] $vault_url,
        [string] $cn = "test.com",
        [string] $ttl = "87600h"
    )

    $root_tokenHeader = RootTokenHeader -root_token $root_token

    Write-Host "Generating Pki Root Certificate"
    Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/pki/root/generate/internal" -Method "Post" -Body "{`"common_name`":`"$cn`",`"ttl`":`"$ttl`"}"

    Write-Host "Set Vault as Certificate Authority"
    Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/pki/config/urls" -Method "Post" -Body "{`"issuing_certificates`":`"$vault_url/v1/pki/ca`", `"crl_distribution_points`":`"$vault_url/v1/pki/crl`"}"

    Write-Host "Get Vault Certificate Authority"
    Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/pki/config/urls" -Method "Get"
}

function CreatePkiCertificateRole
{
    Param(
        [Parameter(Mandatory=$true)] [string] $root_token,
        [Parameter(Mandatory=$true)] [string] $vault_url,
        [Parameter(Mandatory=$true)] [string] $role_name,
        [string] $domains = "test.com",
        [string] $allow_subdomains = "true",
        [string] $max_ttl = "72h"
    )

    $root_tokenHeader = RootTokenHeader -root_token $root_token
    
    Write-Host "Generating Certificate Role $role_name for domains $domains"
    Invoke-WebRequest -Headers $root_tokenHeader -Uri "$vault_url/v1/pki/roles/$role_name" -Method "Post" -Body "{`"allowed_domains`":`"$domains`",`"allow_subdomains`":$allow_subdomains, `"max_ttl`":`"$max_ttl`" }"
}
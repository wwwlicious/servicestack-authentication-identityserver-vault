. "$PSScriptRoot\Vault.ps1"
. "$PSScriptRoot\WebConfig.ps1"

# Service Variables
$vault_url="http://localhost:8200"
$secret_shares=5
$secret_threshold=3

$identity_server_name = "identity-server"

$service_name = "service1"
$service_secrets = "secret1","secret2","secret3","secret4", "secret5"

# Initialises the vault
Write-Host "--1. Initialising Vault-----------------------------------------------------"
$initResponse = Initialise -vault_url $vault_url -secret_shares $secret_shares -secret_threshold $secret_threshold

ConvertTo-Json $initResponse | out-file -filepath "./vault-keys.json"

$root_token=$initResponse.root_token
$keys=$initResponse.keys

# Unseals the vault
Write-Host "--2. Unseal Vault-----------------------------------------------------------"
for($i=0; $i -lt $secret_threshold; $i++) {
    $key = $keys[$i]
    Unseal -vault_url $vault_url -key $key
}

Start-Sleep -m 1000

Write-Host "--3. Mount Transit Backend for Secret Encryption / Decryption---------------"
MountTransitBackend -root_token $root_token -vault_url $vault_url
CreateTransitKey -root_token $root_token -vault_url $vault_url -key $service_name

Write-Host "--4. Mount AppRole Backend for Service / IdentityServer Token Authentication"
MountAppRoleBackend -root_token $root_token -vault_url $vault_url

Write-Host "--5. Mount Pki Backend for Certificate Store--------------------------------"
MountPkiBackend -root_token $root_token -vault_url $vault_url

Write-Host "--5.a Configuring Pki Backend ----------------------------------------------"
TunePkiBackend -root_token $root_token -vault_url $vault_url
GeneratePkiRootCertificate -root_token $root_token -vault_url $vault_url

Write-Host "----------------------------------------------------------------------------"
CreateSecrets -root_token $root_token -vault_url $vault_url -secret_path $service_name -secrets $service_secrets

Write-Host "--7. Setup Identity Server Role---------------------------------------------"
CreatePolicy -root_token $root_token -vault_url $vault_url -policy_name "$service_name-secrets" -path "secret/$service_name" -capabilities @( "create", "read", "update", "list" )
CreatePolicy -root_token $root_token -vault_url $vault_url -policy_name "$service_name-encrypt" -path "transit/encrypt/$service_name" -capabilities @( "create", "update" )
CreateAppRole -root_token $root_token -vault_url $vault_url -app_role $service_name -policies @( "$service_name-secrets", "$service_name-encrypt" )
$service_role_id = CreateAppRoleId -root_token $root_token -vault_url $vault_url -app_role $service_name
Write-Host "AppRoleId for $service_name is $service_role_id"
$service_secret_id = CreateAppRoleSecretId -root_token $root_token -vault_url $vault_url -app_role $service_name
Write-Host "AppRoleSecretId for $service_name is $service_secret_id"

Write-Host "--7.a Seting Service Sample Config Files-------------------------------------"
$appConfigPath = Resolve-Path "../ServiceStack.Vault.SecretStore.ClientDemo/App.Config"
Write-Host "Updating App Settings - Path $appConfigPath"
UpdateAppSetting -web_config_path $appConfigPath -key "ServiceName" -value $service_name
UpdateAppSetting -web_config_path $appConfigPath -key "AppRoleId" -value $service_role_id
UpdateAppSetting -web_config_path $appConfigPath -key "AppSecretId" -value $service_secret_id

$appConfigPath = Resolve-Path "../ServiceStack.Vault.CertStore.ClientDemo/App.Config"
Write-Host "Updating App Settings - Path $appConfigPath"
UpdateAppSetting -web_config_path $appConfigPath -key "ServiceName" -value $service_name
UpdateAppSetting -web_config_path $appConfigPath -key "AppRoleId" -value $service_role_id
UpdateAppSetting -web_config_path $appConfigPath -key "AppSecretId" -value $service_secret_id


Write-Host "--8. Setup Identity Server Role------------------------------------------------"
CreatePkiCertificateRole -root_token $root_token -vault_url $vault_url -role_name $identity_server_name
CreatePolicy -root_token $root_token -vault_url $vault_url -policy_name "$identity_server_name-secrets" -path "secret/$service_name" -capabilities @( "read" )
CreatePolicy -root_token $root_token -vault_url $vault_url -policy_name "$identity_server_name-decrypt" -path "transit/decrypt/$service_name" -capabilities @( "create", "update" )
CreatePolicy -root_token $root_token -vault_url $vault_url -policy_name "$identity_server_name-cert" -path "pki/issue/$identity_server_name" -capabilities @( "create", "read", "update" )

CreateAppRole -root_token $root_token -vault_url $vault_url -app_role $identity_server_name -policies @( "$identity_server_name-secrets", "$identity_server_name-decrypt", "$identity_server_name-cert" )
$identity_service_role_id = CreateAppRoleId -root_token $root_token -vault_url $vault_url -app_role $identity_server_name
Write-Host "AppRoleId for $identity_server_name is $identity_service_role_id"
$identity_secret_id = CreateAppRoleSecretId -root_token $root_token -vault_url $vault_url -app_role $identity_server_name
Write-Host "AppRoleSecretId for $identity_server_name is $identity_secret_id"

Write-Host "--7.a Seting IdentityServer Sample Config File----------------------------------"
$appConfigPath = Resolve-Path "../IdSvr3.Vault.SecretStore.Demo/App.Config"
UpdateAppSetting -web_config_path $appConfigPath -key "ServiceName" -value $service_name
UpdateAppSetting -web_config_path $appConfigPath -key "AppRoleId" -value $identity_service_role_id
UpdateAppSetting -web_config_path $appConfigPath -key "AppSecretId" -value $identity_secret_id

$appConfigPath = Resolve-Path "../IdSvr3.Vault.CertStore.Demo/App.Config"
UpdateAppSetting -web_config_path $appConfigPath -key "ServiceName" -value $service_name
UpdateAppSetting -web_config_path $appConfigPath -key "AppRoleId" -value $identity_service_role_id
UpdateAppSetting -web_config_path $appConfigPath -key "AppSecretId" -value $identity_secret_id

SaveAppSettingsJson -app_settings_path "../IdSvr4.Vault.SecretStore.Demo/appSettings.json" -service_name $service_name -app_role_id $identity_service_role_id -app_secret_id $identity_secret_id
SaveAppSettingsJson -app_settings_path "../IdSvr4.Vault.CertStore.Demo/appSettings.json" -service_name $service_name -app_role_id $identity_service_role_id -app_secret_id $identity_secret_id
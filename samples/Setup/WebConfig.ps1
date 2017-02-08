function UpdateAppSetting
{
    Param(
        [Parameter(Mandatory=$true)] [string] $web_config_path, 
        [Parameter(Mandatory=$true)] [string] $key, 
        [Parameter(Mandatory=$true)] [string] $value
    )

    $webConfig = [xml](Get-Content $web_config_path)
    
    if (($addKey = $webConfig.SelectSingleNode("//appSettings/add[@key = '$key']")))
    {
        Write-Host "Found Key: '$key', updating value to $value"
        $addKey.SetAttribute('value', $value)
    }

    $webConfig.Save($web_config_path)
}

function SaveAppSettingsJson
{
    Param(
        [Parameter(Mandatory=$true)] [string] $app_settings_path, 
        [Parameter(Mandatory=$true)] [string] $service_name, 
        [Parameter(Mandatory=$true)] [string] $app_role_id,
        [Parameter(Mandatory=$true)] [string] $app_secret_id
    )

    $app_settings = @{
        ServiceName = $service_name
        AppRoleId = $app_role_id
        AppSecretId = $app_secret_id            
    }
    
    $app_settings_json = ConvertTo-Json $app_settings | out-file -filepath $app_settings_path
}
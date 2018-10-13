param(
	$websiteFolder = "$PSScriptRoot\Rates.Web",
	$resourceGroup = "rates",
	$functionAppName = "ratesfenton",
	$storageAccount = "ratesfenton",
	$storageAccountContainer = "`$web",
	$applicationInsightsKey = $null
)

function Replace-Text($filePath, $replacementToken, $value) {
	$file = (Get-Content $filePath) -join "`n"
	if ($file.Contains($replacementToken)) {
		$file.Replace($replacementToken, $value) | Out-File $filePath
	}
}

function Get-Properties($file) {
	$extension = [System.IO.Path]::GetExtension($file)
	if ($extension -eq ".html") {
		return @{ContentType = "text/html"}
	}
	return @{}
}

if ($applicationInsightsKey -ne $null) {
	Replace-Text "$websiteFolder\index.html" "<insert instrumentation key>" $applicationInsightsKey
}

# Upload static files to storage
Set-AzureRmCurrentStorageAccount -ResourceGroupName $resourceGroup -AccountName $storageAccount
$files = Get-ChildItem $websiteFolder -File -Recurse
Write-Host "Uploading $($files.Count) to $storageAccountContainer"
foreach($file in $files) {
	$fileName = $file.FullName
	$properties = Get-Properties $fileName
	Set-AzureStorageBlobContent `
		-Container $storageAccountContainer `
		-File $fileName `
		-Properties $properties `
		-Force
}

# Deploy functions app
$creds = Invoke-AzureRmResourceAction -ResourceGroupName $resourceGroup `
	-ResourceType Microsoft.Web/sites/config `
    -ResourceName $functionAppName/publishingcredentials `
	-Action list -ApiVersion 2015-08-01 -Force
$username = $creds.Properties.PublishingUserName
$password = $creds.Properties.PublishingPassword
$base64AuthInfo = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(("{0}:{1}" -f $username,$password)))
$apiUrl = "https://$functionAppName.scm.azurewebsites.net/api/zip/site/wwwroot"
$filePath = (Get-Item "$PSScriptRoot\Rates.Functions.*.zip").FullName
$userAgent = "powershell/1.0"
Write-Host "Deploying $filePath"
Invoke-RestMethod -Uri $apiUrl `
	-Headers @{Authorization=("Basic {0}" -f $base64AuthInfo)} `
	-UserAgent $userAgent `
	-Method POST `
	-InFile $filePath `
	-ContentType "multipart/form-data"
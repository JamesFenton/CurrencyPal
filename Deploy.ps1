param(
	$websiteFolder = "$PSScriptRoot\Rates.Web",
	$storageAccountResourceGroup = "rates",
	$storageAccount = "ratesfenton",
	$storageAccountContainer = "`$web",
	$applicationInsightsKey = $null
)

function Replace-Text($filePath, $replacementToken, $value) {
	$file = (Get-Content $filePath) -join "`n"
	$file.Replace($replacementToken, $value) | Out-File $filePath
}

if ($applicationInsightsKey -ne $null) {
	Replace-Text "$websiteFolder\index.html" "<insert instrumentation key>" $applicationInsightsKey
}

# Upload static files to storage
Set-AzureRmCurrentStorageAccount -ResourceGroupName $storageAccountResourceGroup -AccountName $storageAccount

$files = Get-ChildItem $websiteFolder -File -Recurse
Write-Host "Uploading $($files.Count) to $storageAccountContainer"
$files | Set-AzureStorageBlobContent -Container $storageAccountContainer -Force

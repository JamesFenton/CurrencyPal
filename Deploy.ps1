param(
	$websiteFolder = "$PSScriptRoot\Rates.Web",
	$storageAccountResourceGroup = "rates",
	$storageAccount = "ratesfenton",
	$storageAccountContainer = "`$web"

# Upload static files to storage
Set-AzureRmCurrentStorageAccount -ResourceGroupName $storageAccountResourceGroup -AccountName $storageAccount

$files = Get-ChildItem $websiteFolder -File -Recurse
Write-Host "Uploading $($files.Count) to $storageAccountContainer"
$files | Set-AzureStorageBlobContent -Container $storageAccountContainer

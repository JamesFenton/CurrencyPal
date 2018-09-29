param(
	$websiteFolder = "$PSScriptRoot\Rates.Web"
)

# Upload static files to storage
Set-AzureRmCurrentStorageAccount -ResourceGroupName "rates" -AccountName "ratesfenton"
Get-ChildItem $websiteFolder -File -Recurse | Set-AzureStorageBlobContent -Container "`$web"

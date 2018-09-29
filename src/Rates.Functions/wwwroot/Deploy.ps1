Set-AzureRmCurrentStorageAccount -ResourceGroupName "rates" -AccountName "ratesfenton"
Get-ChildItem "$PSScriptRoot" -File -Recurse | Set-AzureStorageBlobContent -Container "`$web"

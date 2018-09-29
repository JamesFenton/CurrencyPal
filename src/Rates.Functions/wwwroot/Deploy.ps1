Set-AzureRmCurrentStorageAccount -ResourceGroupName "rates" -AccountName "ratesfenton"
Get-ChildItem -File -Recurse | Set-AzureStorageBlobContent -Container "`$web"

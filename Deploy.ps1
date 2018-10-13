param(
	$websiteFolder = "$PSScriptRoot\Rates.Web",
	$storageAccountResourceGroup = "rates",
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
Set-AzureRmCurrentStorageAccount -ResourceGroupName $storageAccountResourceGroup -AccountName $storageAccount

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

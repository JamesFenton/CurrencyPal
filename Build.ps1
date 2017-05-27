$version = $env:APPVEYOR_BUILD_VERSION
$webPublishDirectory = "$PSScriptRoot\publish\web"

dotnet restore

dotnet publish -o $webPublishDirectory -c Release
$version | Out-Host -File "$webPublishDirectory\version.txt"

7z a "Rates.Web.$version.zip" .\publish\web\**
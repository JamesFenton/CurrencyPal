param(
	$buildVersion = "0.0.1",
	$buildCounter = 0,
	$artifactDirectory = "$PSScriptRoot\artifacts"
)

function Test-ExitCode($exitCode) {
	if ($exitCode -ne 0) {
		exit $exitCode
	}
}

$version = "$buildVersion.$buildCounter"
$functionsPublishDirectory = "$PSScriptRoot\publish\Rates.Functions"

# publish functions
dotnet publish "$PSScriptRoot\src\Rates.Functions" -o $functionsPublishDirectory -c Release
Test-ExitCode $lastExitCode

# create artifact directory
if (Test-Path $artifactDirectory) {
	Remove-Item $artifactDirectory -Recurse -Force
}
New-Item -ItemType Directory -Path $artifactDirectory

# zip functions
Compress-Archive $functionsPublishDirectory\** "$artifactDirectory\Rates.Functions.$version.zip"
Test-ExitCode $lastExitCode

# zip website
$websiteDirectory = "$PSScriptRoot\src\Rates.Web\wwwroot"
Copy-Item "$websiteDirectory\settings.prod.js" "$websiteDirectory\settings.js" -Force
Copy-Item "$websiteDirectory\**" "$artifactDirectory\Rates.Web" -Recurse

Copy-Item "$PSScriptRoot\Deploy.ps1" $artifactDirectory

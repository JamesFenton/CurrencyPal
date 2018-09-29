param(
	$buildVersion = $env:APPVEYOR_BUILD_VERSION,
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

# restore
dotnet restore
Test-ExitCode $lastExitCode

# build
dotnet build --configuration Release
Test-ExitCode $lastExitCode

# publish web
dotnet publish "$PSScriptRoot\src\Rates.Functions" -o $functionsPublishDirectory -c Release
Test-ExitCode $lastExitCode

# create artifact directory
if (Test-Path $artifactDirectory) {
	Remove-Item $artifactDirectory -Force
}
New-Item -ItemType Directory -Path $artifactDirectory

# zip functions
Compress-Archive $functionsPublishDirectory\** "$artifactDirectory\Rates.Functions.$version.zip"
Test-ExitCode $lastExitCode

# zip front-end
Compress-Archive "$PSScriptRoot\src\Rates.Functions\wwwroot\**" "$artifactDirectory\Rates.Web.$version.zip"

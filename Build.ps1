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
$webPublishDirectory = "$PSScriptRoot\publish\Rates.Web"
$servicePublishDirectory = "$PSScriptRoot\src\Rates.Fetcher\bin\Release\net461"

# restore
dotnet restore
Test-ExitCode $lastExitCode

# build
dotnet build --configuration Release
Test-ExitCode $lastExitCode

# publish web
dotnet publish "$PSScriptRoot\src\Rates.Web" -o $webPublishDirectory -c Release
Test-ExitCode $lastExitCode
$version | Out-File "$webPublishDirectory\version.txt"

# create artifact directory
if (-not (Test-Path $artifactDirectory)) {
	New-Item -ItemType Directory -Path $artifactDirectory
}

# zip web
Compress-Archive $webPublishDirectory\** "$artifactDirectory\Rates.Web.$version.zip"
Test-ExitCode $lastExitCode

# zip fetcher
Compress-Archive $servicePublishDirectory\** "$artifactDirectory\Rates.Fetcher.$version.zip"
Test-ExitCode $lastExitCode

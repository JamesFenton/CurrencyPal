param(
	$version = $env:APPVEYOR_BUILD_VERSION,
	$outputDirectory = "$PSScriptRoot\publish\web"
)

function Test-ExitCode($exitCode) {
	if ($exitCode -ne 0) {
		exit $exitCode
	}
}

Write-Host "Running build for version $version"

# restore
dotnet restore
Test-ExitCode $lastExitCode

# build
dotnet build --configuration Release
Test-ExitCode $lastExitCode

# publish web
dotnet publish "$PSScriptRoot\src\Rates.Web" -o $outputDirectory -c Release
Test-ExitCode $lastExitCode
$version | Out-File "$webPublishDirectory\version.txt"

# zip web
7z a "Rates.Web.$version.zip" .\publish\web\**
Test-ExitCode $lastExitCode

# zip fetcher
7z a "Rates.Fetcher.$version.zip" .\src\Rates.Fetcher\bin\Release\net461\**
Test-ExitCode $lastExitCode

param(
	$version = $env:APPVEYOR_BUILD_VERSION,
	$outputDirectory = "$PSScriptRoot\publish\web"
)

function Test-ExitCode($exitCode) {
	if ($exitCode -ne 0) {
		exit $exitCode
	}
}

# restore
dotnet restore
Test-ExitCode $lastExitCode

# build
dotnet build --configuration Release
Test-ExitCode $lastExitCode

# publish web
dotnet publish "$PSScriptRoot\publish\web" -o $outputDirectory -c Release
Test-ExitCode $lastExitCode
$version | Out-File "$webPublishDirectory\version.txt"

# zip web
7z a "Rates.Web.$version.zip" .\publish\web\**
Test-ExitCode $lastExitCode

# zip fetcher
7z a "Rates.Fetcher.$version.zip" .\src\Rates.Fetcher\bin\Release\net461\**
Test-ExitCode $lastExitCode

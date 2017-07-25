$version = $env:APPVEYOR_BUILD_VERSION

function Test-ExitCode($exitCode) {
	if ($exitCode -ne 0) {
		exit $exitCode
	}
}

# restore
dotnet restore
Test-ExitCode $lastExitCode

# build
dotnet build
Test-ExitCode $lastExitCode

# publish web
$webPublishDirectory = "$PSScriptRoot\publish\web"
Push-Location "$PSScriptRoot\src\Rates.Web"
dotnet publish -o $webPublishDirectory -c Release
Test-ExitCode $lastExitCode
$version | Out-File "$webPublishDirectory\version.txt"
Pop-Location

# zip web
7z a "Rates.Web.$version.zip" .\publish\web\**
Test-ExitCode $lastExitCode

# zip fetcher
7z a "Rates.Fetcher.$version.zip" .\src\Rates.Fetcher\bin\Debug\net461\win7-x86\**
Test-ExitCode $lastExitCode

dotnet restore

dotnet publish -o "$PSScriptRoot\publish\web" -c Release

7z a Rates.Web.zip .\publish\web\**
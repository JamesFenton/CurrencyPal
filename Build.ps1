dotnet restore

dotnet publish -o "$PSScriptRoot\publish\web"

7z a Rates.Web.zip .\publish\\web**
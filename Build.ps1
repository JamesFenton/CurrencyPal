dotnet restore

Push-Location "$PSScriptRoot\src\CurrencyPal"
yarn
npm run build
Pop-Location

dotnet publish "$PSScriptRoot\src\CurrencyPal" -o "$PSScriptRoot\publish"

7z a Rates.zip .\publish\**
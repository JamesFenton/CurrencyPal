dotnet restore

dotnet publish "$PSScriptRoot\src\CurrencyPal" -o "$PSScriptRoot\publish"

Push-Location "$PSScriptRoot\src\CurrencyPal"
yarn
npm run build
Pop-Location

7z a Rates.zip .\publish\**
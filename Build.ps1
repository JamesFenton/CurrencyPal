dotnet restore

dotnet publish "$PSScriptRoot\src\CurrencyPal" -o "$PSScriptRoot\publish"

Push-Location "$PSScriptRoot\src\CurrencyPal"
yarn
npm install aurelia-cli -g
au build
Pop-Location

7z a Rates.zip .\publish\**
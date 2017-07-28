# This is required because the AppVeyor deployment agent fails to deploy the website:
# Web Deploy cannot modify the file 'Rates.Web.exe' on the destination because it is locked by an external process

Stop-Website Rates
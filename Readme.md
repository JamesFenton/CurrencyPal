# Rates

[![Build Status](https://dev.azure.com/jfenton/JamesApps/_apis/build/status/Rates?branchName=master)](https://dev.azure.com/jfenton/JamesApps/_build/latest?definitionId=1&branchName=master)
[![Release Status](https://vsrm.dev.azure.com/jfenton/_apis/public/Release/badge/4ccaff51-2a04-4919-85ac-9e048c5dde4c/2/2)](https://dev.azure.com/jfenton/JamesApps/_release?view=all&definitionId=2)


A simple tool to show exchange rates in the format I want built using Azure Functions and Azure Storage.

## Overview
The app is split into two parts
- Write Model
- Read Model

### Write Model
The list of rates to fetch is stored in Blob storage:
/lookups/rates.json

There are a few functions that fetch rate information on a timer trigger, with 1 function per data source. Each function:
- Fetches the applicable rates
- The rates are saved to Azure Table Storage in the table `rates`
- For each new rate, a queue message is sent to the Storage queue `rates-added`

### Read Model
The function `RateAddedHandler` listens for messages on the storage queue `rates-added`
When it recieves an new rate, it recalculates the various % changes, eg 1 week, 1 month, etc, from the historical data in the Write Model, 
and updates it's row in the table `ratesrm`

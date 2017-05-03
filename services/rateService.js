const fetch = require("node-fetch");

const tickers = ["USDZAR", "GBPZAR", "ZARMUR", "BTCZAR", "BTCUSD"];

var cacheExpiryTime = Date.now();
var cachedRates;

var getRates = function() {

    // if the cached rates are still valid
    if (Date.now() < cacheExpiryTime) {
        return Promise.resolve(cachedRates)
    }

    // get rates
    var bitcoinPromise = getBitcoin();
    var otherRatesPromise = getOtherRates();

    return Promise.all([bitcoinPromise, otherRatesPromise])
        .then(results => {
            var bitcoin = results[0];
            var otherRates = results[1];
            var rates = tickers.map(ticker => ticker === "BTCZAR" ? bitcoin : otherRates.find(r => r.ticker === ticker));
            
            updateTime = Date.now();
            cacheExpiryTime = updateTime + 1000 * 60 * 60 // 1 hour from now
            cachedRates = {
                rates: rates,
                updateTime: updateTime
            };

            return cachedRates;
        })
}

var getBitcoin = function() {
    return fetch('https://api.mybitx.com/api/1/ticker?pair=XBTZAR')
        .then(res => res.json())
        .then(res => {
            var r = {ticker: "BTCZAR", rate: parseFloat(res.last_trade)};
            return r;
        });
}

var getOtherRates = function() {
    var appId = process.env.OPENEXCHANGERATES_APPID;
    return fetch("https://openexchangerates.org/api/latest.json?app_id=" + appId)
        .then(res => res.json())
        .then(res => {
            var source = res.rates;
            var rates = tickers.map(x => convertRate(x, source));
            return rates;
        });
}

var convertRate = function(ticker, rates){
    var source = ticker.substring(0, 3);
    var dest = ticker.substring(3, 6);

    var sourceRate = rates[source];
    var destRate = rates[dest];
    var crossRate = destRate / sourceRate;
    return {ticker: ticker, rate: crossRate};
}

module.exports = getRates;
var rp = require('request-promise-native');

const tickers = ["USDZAR", "GBPZAR", "ZARMUR", "BTCZAR", "BTCUSD"];

var getRates = function() {

    var bitcoinPromise = getBitcoin();
    var otherRatesPromise = getOtherRates();

    return Promise.all([bitcoinPromise, otherRatesPromise])
        .then(results => {
            var bitcoin = results[0];
            var otherRates = results[1];
            var rates = tickers.map(ticker => ticker === "BTCZAR" ? bitcoin : otherRates.find(r => r.ticker === ticker));
            return rates;
        })
}

var getBitcoin = function() {
    var options = {
        uri: 'https://api.mybitx.com/api/1/ticker?pair=XBTZAR',
        json: true // Automatically parses the JSON string in the response 
    };
    return rp(options)
        .then(res => {
            var r = {ticker: "BTCZAR", rate: parseFloat(res.last_trade)};
            return r;
        });
}

var getOtherRates = function() {
    var appId = process.env.CURRENCYLAYER_APPID;
    var options = {
        uri: "https://openexchangerates.org/api/latest.json?app_id=" + appId,
        json: true // Automatically parses the JSON string in the response 
    };
    return rp(options)
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
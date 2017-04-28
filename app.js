var express = require('express')
var app = express()

var getRates = require("./services/rateService") 

var port = process.env.PORT || 3000

app.get('/api/rates', function (req, res) {
  getRates()
    .then(rates => res.send(rates))
})

app.listen(port, function () {
  console.log('App listening on port ' + port)
})

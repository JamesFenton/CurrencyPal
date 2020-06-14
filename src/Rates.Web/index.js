import {RatesService} from "./rates-service.js";

const ratesService = new RatesService();

var app = new Vue({
    el: '#app',
    data: {
        rates: [],
        updateTimeMessage: null,
        nextUpdateTime: 0,
        nextUpdateTimeMessage: null,
        changePeriod: null,
        requesting: false,
        errorMessage: null
    },
    methods: {
        getRates: function () {
            this.requesting = true;
            this.errorMessage = null;
            ratesService.getRates()
                .then(dto => {
                    this.requesting = false;

                    this.rates = dto.rates;

                    this.rates.forEach(r => 
                        r.updateTimeMessage = r.outOfDate ? "Last updated at " + new Date(r.timestamp) : null
                    );
                })
                .catch(error => {
                    var response = error.response.data;
                    this.requesting = false;

                    this.errorMessage = response.message;
                });
        }
    },
    watch: {
        changePeriod: function (newValue) {
            window.localStorage.setItem("changePeriod", this.changePeriod);
        }
    },
    filters: {
        round: function (value) {
            if (value === null)
                return '';
            if (value < 1)
                return value.toFixed(4);
            return value.toFixed(2);
        },
        roundedPercentage: function (value) {
            if (value === null)
                return '';
            var rounded = value.toFixed(2) + '%';
            return rounded;
        }
    },
    created: function () {
        this.getRates();
        this.changePeriod = window.localStorage.getItem("changePeriod");
        if (!this.changePeriod)
            this.changePeriod = "1w";
        document.getElementById("app").removeAttribute("hidden");
    }
});
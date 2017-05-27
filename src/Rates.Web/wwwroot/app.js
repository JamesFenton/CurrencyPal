var app = new Vue({
    el: '#app',
    data: {
        rates: [],
        updateTimeMessage: null,
        nextUpdateTime: 0,
        nextUpdateTimeMessage: null,
        updateAvailable: false,
        requesting: false
    },
    methods: {
        getRates: function () {
            this.requesting = true;
            fetch("api/rates")
                .then(res => res.json())
                .then(dto => {
                    this.requesting = false;
                    this.rates = dto.rates;
                    this.updateTimeMessage = "Last updated at " + new Date(dto.updateTime).toLocaleTimeString();
                    this.nextUpdateTime = dto.nextUpdateTime;
                    this.nextUpdateTimeMessage = "Next update at " + new Date(dto.nextUpdateTime).toLocaleTimeString();
                    this.isUpdateAvailable();
                })
                .catch(() => this.requesting = false);
        },
        isUpdateAvailable: function () {
            this.updateAvailable = Date.now() > this.nextUpdateTime;
        }
    },
    filters: {
        round: function (value) {
            if (!value) 
                return '';
            var rounded = value.toFixed(2);
            return rounded;
        }
    },
    created: function() {
        this.getRates();
        this.isUpdateAvailable();
        setInterval(() => this.isUpdateAvailable(), 5000);
    }
})
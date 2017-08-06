var app = new Vue({
    el: '#app',
    data: {
        rates: [],
        updateTimeMessage: null,
        nextUpdateTime: 0,
        nextUpdateTimeMessage: null,
        requesting: false,
        errorMessage: null
    },
    methods: {
        getRates: function () {
            this.requesting = true;
            this.errorMessage = null;
            axios.get("api/rates")
                .then(response => {
                    var dto = response.data;
                    this.requesting = false;

                    this.rates = dto.rates;
                    this.updateTimeMessage = "Last updated at " + new Date(dto.updateTime).toLocaleTimeString();
                })
                .catch(error => {
                    var response = error.response.data;

                    this.requesting = false
                    this.errorMessage = response.message;
                });
        }
    },
    filters: {
        round: function (value) {
            if (!value) 
                return '';
            if (value < 1)
                return value.toFixed(4);
            return value.toFixed(2);
        },
        roundedPercentage: function (value) {
            if (!value)
                return '';
            var rounded = (value * 100).toFixed(2) + '%';
            return rounded;
        }
    },
    created: function() {
        this.getRates();
    }
})
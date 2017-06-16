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
                    this.nextUpdateTime = dto.nextUpdateTime;
                    this.nextUpdateTimeMessage = "Next update at " + new Date(dto.nextUpdateTime).toLocaleTimeString();
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
            var rounded = value.toFixed(2);
            return rounded;
        }
    },
    created: function() {
        this.getRates();
    }
})
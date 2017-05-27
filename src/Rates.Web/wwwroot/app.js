var app = new Vue({
    el: '#app',
    data: {
        rates: [],
        updateTime: null
    },
    methods: {
        getRates: function() {
            fetch("api/rates")
                .then(res => res.json())
                .then(dto => {
                    this.rates = dto.rates;
                    var updated = new Date(dto.updateTime);
                    this.updateTime = "Last updated at " + updated.toLocaleTimeString();
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
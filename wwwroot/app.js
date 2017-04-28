var app = new Vue({
    el: '#app',
    data: {
        rates: []
    },
    methods: {
        getRates: function() {
            fetch("api/rates")
                .then(res => res.json())
                .then(rates => this.rates = rates);
        }
    },
    filters: {
        round: function (value) {
            if (!value) 
                return '';
            var rounded = Math.round(value * 100) / 100;
            return rounded;
        }
    },
    created: function() {
        this.getRates();
    }
})
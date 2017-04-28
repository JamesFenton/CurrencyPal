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
            var rounded = value.toFixed(2);
            return rounded;
        }
    },
    created: function() {
        this.getRates();
    }
})
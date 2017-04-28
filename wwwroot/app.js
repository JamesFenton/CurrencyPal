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
  created: function() {
    this.getRates();
  }
})
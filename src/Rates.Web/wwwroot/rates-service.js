import settings from "./settings.js";

export class RatesService {
    
    constructor() {
        this.ratesUrl = settings.devMode
            ? "/example-rates.json"
            : "https://ratesfenton.azurewebsites.net/api/rates";
    }

    getRates() {
        const isOutOfDate = (timestamp) => {
            const oneHourAgo = new Date(new Date() - 1000 * 60 * 60);
            return new Date(timestamp) < oneHourAgo;
        };

        return fetch(this.ratesUrl)
            .then(res => res.json())
            .then(res => {
                res.rates.forEach(r => r.outOfDate = isOutOfDate(r.timestamp));
                return res;
            });
    }

    private 
}

export default new RatesService();
import settings from "./settings.js";

export class RatesService {
    
    constructor() {
        this.ratesUrl = settings.devMode
            ? "/example-rates.json"
            : "https://ratesfenton.azurewebsites.net/api/rates";
    }

    getRates() {
        return fetch(this.ratesUrl)
            .then(res => res.json());
    }
}

export default new RatesService();
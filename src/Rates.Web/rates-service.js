import settings from "./settings.js";

export class RatesService {

    getRates() {
        const isOutOfDate = (timestamp) => {
            const oneHourAgo = new Date(new Date() - 1000 * 60 * 60);
            return new Date(timestamp) < oneHourAgo;
        };

        return fetch(settings.ratesUrl)
            .then(res => res.json())
            .then(res => {
                res.rates.forEach(r => r.outOfDate = isOutOfDate(r.timestamp));
                return res;
            });
    }
}

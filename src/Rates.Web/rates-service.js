import settings from "./settings.js";
import exampleRates from "./example-rates.json";

class RatesService {

    getRates() {
        const fetchExampleRates = () => {
            if (settings.useExampleRates) {
                return new Promise((resolve, reject) => {
                    setTimeout(() => resolve(exampleRates), 2000);
                });
            }
        }

        const fetchRates = () => {
            return fetch(settings.ratesUrl)
                .then(res => res.json());
        }

        return settings.useExampleRates
            ? fetchExampleRates()
            : fetchRates();
    }
}

export default new RatesService();

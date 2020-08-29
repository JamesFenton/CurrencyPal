import settings from "./settings";
import exampleRates from "./example-rates.json";

class RatesService {
  getRates() {
    const fetchExampleRates = () => {
      if (settings.useExampleRates) {
        return new Promise((resolve, reject) => {
          setTimeout(() => resolve(exampleRates), 2000);
        });
      }
    };

    const fetchRates = () => {
      return fetch("/api/rates").then((res) => res.json());
    };

    return settings.useExampleRates ? fetchExampleRates() : fetchRates();
  }
}

export default new RatesService();

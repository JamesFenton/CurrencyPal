import { autoinject } from "aurelia-framework";
import { HttpClient } from "aurelia-http-client";
import { RatesDto } from "./models";

@autoinject
export class App {
    rates: RatesDto;

    constructor(private readonly client: HttpClient) { }

    activate() {
        this.client.get("/api/rates")
            .then(res => this.rates = res.content);

    }
}

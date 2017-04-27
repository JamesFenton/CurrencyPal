export class RatesDto {
    rates: Rate[];
}

export class Rate {
    ticker: string;
    rate: number;
}
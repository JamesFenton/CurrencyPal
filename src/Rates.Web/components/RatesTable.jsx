import React, {Component} from 'react';
import RatesRow from "./RatesRow";
import ratesService from "../rates-service";

class RatesTable extends Component {
    state = {
        rates: [],
        selectedTimePeriod: window.localStorage.getItem("changePeriod") || '1w',
        requesting: false
    };

    componentDidMount() {
        this.getRates();
    }

    getRates() {
        this.setState({requesting: true});
        ratesService.getRates()
            .then(dto => {
                this.setState({
                    rates: dto.rates,
                    requesting: false,
                });
            })
            .catch(err => {
                // todo error message
            })
    }

    changeSelectedTimePeriod = (e) => {
        this.setState({selectedTimePeriod: e.target.value});
        window.localStorage.setItem("changePeriod", this.state.selectedTimePeriod);
    }

    update = () => {
        this.getRates();
    }

    render() {
        const timePeriodOptions = ['1w', '1m', '3m', '6m', '1y'];

        return (
            <div className="card">
                <div className="card-body">
                    <h5 className="card-title">Exchange Rates</h5>
                    
                    <table className="table table-sm">
                    <thead>
                        <tr>
                            <th scope="col">Ticker</th>
                            <th scope="col">Rate</th>
                            <th scope="col">1d</th>
                            <th>
                                <select className="form-control form-control-sm" onChange={this.changeSelectedTimePeriod}>
                                    {timePeriodOptions.map(o => 
                                        <option selected={o === this.state.selectedTimePeriod}>
                                            {o}
                                        </option>    
                                    )}
                                </select>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        {this.state.rates.map(rate => 
                            <RatesRow key={rate.ticker} rate={rate} rateChange={this.getRateChange(rate)} />
                        )}
                    </tbody>
                    </table>

                <button className="btn btn-primary" onClick={this.update} disabled={this.state.requesting}>
                    {this.state.requesting ? 'Updating...' : 'Update'}
                </button>

                </div>
            </div>
        );
    }

    
    getRateChange(rate) {
        switch(this.state.selectedTimePeriod) {
            case '1w':
                return rate.change1Week;
            case '1m':
                return rate.change1Month;
            case '3m':
                return rate.change3Months;
            case '6m':
                return rate.change6Months;
            case '1y':
                return rate.change1Year;
        }
    }
}

export default RatesTable;
import React, {Component} from 'react';
import RatesRow from "./RatesRow";
import ratesService from "../rates-service";

class RatesTable extends Component {
    constructor(props) {
        super(props);
        this.state = {
            rates: [],
            selectedTimePeriod: window.localStorage.getItem("changePeriod") || '1w',
            requesting: false
        };

        this.update = this.update.bind(this);
        this.changePeriod = this.changeSelectedTimePeriod.bind(this);
    }

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

    update() {
        this.getRates();
    }

    changeSelectedTimePeriod(e) {
        this.setState({selectedTimePeriod: e.target.value});
        window.localStorage.setItem("changePeriod", this.state.selectedTimePeriod);
    }

    render() {
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
                                    <option>1w</option>
                                    <option>1m</option>
                                    <option>3m</option>
                                    <option>6m</option>
                                    <option>1y</option>
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
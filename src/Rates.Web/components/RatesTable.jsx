import React, { Component } from "react";
import RatesRow from "./RatesRow";
import ratesService from "../rates-service";

class RatesTable extends Component {
  state = {
    rates: [],
    requesting: false,
  };

  componentDidMount() {
    this.getRates();
  }

  getRates() {
    this.setState({ requesting: true });
    ratesService
      .getRates()
      .then((dto) => {
        this.setState({
          rates: dto.rates,
          requesting: false,
        });
      })
      .catch((err) => {
        // todo error message
      });
  }

  update = () => {
    this.getRates();
  };

  render() {
    return (
      <div className="card">
        <div className="card-body">
          <h5 className="card-title">Exchange Rates</h5>

          <table className="table table-sm table-hover">
            <thead>
              <tr>
                <th scope="col">Ticker</th>
                <th scope="col">Rate</th>
                <th scope="col">1d</th>
                <th scope="col">1w</th>
                <th scope="col">1m</th>
                <th scope="col">3m</th>
                <th scope="col">6m</th>
                <th scope="col">1y</th>
              </tr>
            </thead>
            <tbody>
              {this.state.rates.map((rate) => (
                <RatesRow key={rate.ticker} rate={rate} />
              ))}
            </tbody>
          </table>

          <button
            className="btn btn-primary"
            onClick={this.update}
            disabled={this.state.requesting}
          >
            {this.state.requesting ? "Updating..." : "Update"}
          </button>
        </div>
      </div>
    );
  }

  getRateChange(rate) {
    switch (this.state.selectedTimePeriod) {
      case "1w":
        return rate.change1Week;
      case "1m":
        return rate.change1Month;
      case "3m":
        return rate.change3Months;
      case "6m":
        return rate.change6Months;
      case "1y":
        return rate.change1Year;
    }
  }
}

export default RatesTable;

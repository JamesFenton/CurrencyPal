import React, {Component} from 'react';
import RatesValue from "./RatesValue";

class RatesRow extends Component {

    render() {
        const {rate, rateChange} = this.props;
        const outOfDate = this.isOutOfDate(rate.timestamp);
        const updateMessage = outOfDate ? "Last updated at " + new Date(rate.timestamp) : null;
        return (
            <tr>
                <td className={outOfDate ? 'table-danger' : ''} title={updateMessage}>
                    {rate.href ? <a href={rate.href} target="_blank">{rate.name}</a> : <span>{rate.name}</span>}
                </td>
                <td>{this.round(rate.value)}</td>
                <RatesValue rate={rate.change1Day} />
                <RatesValue rate={rateChange} />
            </tr>
        )
    }

    round(value) {
        if (!value)
            return '';
        if (value < 1)
            return value.toFixed(4);
        return value.toFixed(2);
    }

    isOutOfDate(timestamp) {
        const oneHourAgo = new Date(new Date() - 1000 * 60 * 60);
        return new Date(timestamp) < oneHourAgo;
    }
}

export default RatesRow;
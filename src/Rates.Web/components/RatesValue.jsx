import React, {Component} from 'react';

class RatesValue extends Component {

    render() {
        const {rate} = this.props;

        return (
            <td style={this.getStyle(rate)}>
                {this.roundedPercentage(rate)}
            </td>
        )
    }

    getStyle(rate) {
        if (rate === 0)
            return null;
        if (rate > 0)
            return {color: 'MediumSeaGreen'};
        else if (rate < 0)
            return {color: 'Tomato'};
    }

    roundedPercentage(value) {
        if (value === 0)
            return '0.00%';
        if (!value)
            return '';
        return value.toFixed(2) + '%';
    }
}

export default RatesValue;
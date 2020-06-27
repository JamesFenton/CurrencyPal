import React, {Component} from 'react';

class RatesValue extends Component {

    render() {
        const {rate} = this.props;

        return (
            <td style={rate > 0 ? {color: 'MediumSeaGreen'} : {color: 'Tomato'}}>
                {this.roundedPercentage(rate)}
            </td>
        )
    }

    roundedPercentage(value) {
        if (!value)
            return '';
        return value.toFixed(2) + '%';
    }
}

export default RatesValue;
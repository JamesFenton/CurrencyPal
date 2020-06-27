import React, {Component} from 'react';
import RatesTable from "./components/RatesTable";

class App extends Component {

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-3"></div>

                    <div className="col-md-6">
                        <RatesTable />
                    </div>
                    
                    <div className="col-md-3"></div>
                </div>
            </div>
        )
    }
}

export default App;
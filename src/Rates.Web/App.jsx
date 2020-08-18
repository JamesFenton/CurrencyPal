import React from "react";
import RatesTable from "./components/RatesTable";

export default function () {
  return (
    <div className="container">
      <div className="row">
        <div className="col-md-2"></div>

        <div className="col-md-8">
          <RatesTable />
        </div>

        <div className="col-md-2"></div>
      </div>
    </div>
  );
}

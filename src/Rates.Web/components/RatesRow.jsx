import React from "react";
import RatesValue from "./RatesValue";

export default function ({ rate }) {
  const round = (value) => {
    if (!value) return "";
    if (value < 1) return value.toFixed(4);
    if (value < 100) return value.toFixed(2);
    return value.toFixed(0);
  };

  const isOutOfDate = (timestamp) => {
    const oneHourAgo = new Date(new Date() - 1000 * 60 * 60);
    return new Date(timestamp) < oneHourAgo;
  };

  const outOfDate = isOutOfDate(rate.timestamp);
  const updateMessage = outOfDate
    ? "Last updated at " + new Date(rate.timestamp)
    : null;

  return (
    <tr>
      <td className={outOfDate ? "table-danger" : ""} title={updateMessage}>
        {rate.href ? (
          <a href={rate.href} target="_blank">
            {rate.name}
          </a>
        ) : (
          <span>{rate.name}</span>
        )}
      </td>
      <td>{round(rate.value)}</td>
      <RatesValue rate={rate.change1Day} />
      <RatesValue rate={rate.change1Week} />
      <RatesValue rate={rate.change1Month} />
      <RatesValue rate={rate.change3Months} />
      <RatesValue rate={rate.change6Months} />
      <RatesValue rate={rate.change1Year} />
    </tr>
  );
}

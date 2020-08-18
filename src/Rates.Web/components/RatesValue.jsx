import React from "react";

export default function ({ rate }) {
  const roundedPercentage = (value) => {
    if (value === 0) return "0.00%";
    if (!value) return "";
    return value.toFixed(2) + "%";
  };

  const getStyle = (rate) => {
    if (rate === 0) return null;
    if (rate > 0) return { color: "MediumSeaGreen" };
    else if (rate < 0) return { color: "Tomato" };
  };

  return <td style={getStyle(rate)}>{roundedPercentage(rate)}</td>;
}

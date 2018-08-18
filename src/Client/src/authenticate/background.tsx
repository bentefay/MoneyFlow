import * as React from "react";
import { css } from "emotion";

export const c = {
  background: css({
    position: "fixed",
    zIndex: -1,
    top: "0",
    left: "0",
    right: "0",
    bottom: "0",
    backgroundColor: "#203B46"
  }),
  gray1: css({
    fill: "#23404C"
  }),
  gray2: css({
    fill: "#264653"
  }),
  orange1: css({
    fill: "#E76F51"
  }),
  orange2: css({
    fill: "#F4A261"
  })
};

export const Background = () => (
  <svg
    className={c.background}
    xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 100 100"
    width="100%"
    height="100%"
    preserveAspectRatio="none"
  >
    <path className={c.orange2} d={path(100, 40, 0, 1)} />
    <path className={c.gray2} d={path(100, 30, 0, 1)} />

    <path className={c.orange1} d={path(160, 100, 57, 1)} />
    <path className={c.orange1} d={path(170, -90, -26, 1)} />

    <path className={c.gray1} d={path(160, -90, -25, 1)} />
    <path className={c.gray1} d="M0 49.9 H 100 V 50.1 H 0" />
  </svg>
);

function path(
    period: number,
    amplitude: number,
    phase: number,
    repeat: number
  ) {
    const line = new Array(repeat)
      .fill(0)
      .map((_, i) => i)
      .map(i => {
        const offset = phase + period * i;
        return `Q ${offset + period / 4} ${50 - amplitude}, ${offset +
          period / 2} 50 T ${offset + period} 50`;
      })
      .join(" ");
    return `M${phase} 50 ${line}`;
  }
  
import "tslib";
import * as React from "react";
import { render } from "react-dom";
import { account, background, blue, orange, gray, brand, first, second, form, label, input, button, blue2 } from './index.style';

const Root = () => (
  <div className={account}>
    <svg
      className={background}
      xmlns="http://www.w3.org/2000/svg"
      viewBox="0 0 100 100"
      width="100%"
      height="100%"
      preserveAspectRatio="none">
      <path className={blue} d={path(100, 40, 0, 1)} />
      <path className={blue2} d={path(100, 30, 0, 1)} />
      <path className={orange} d={path(160, 100, 57, 1)} />
      <path className={orange} d={path(170, -90, -26, 1)} />
      <path className={gray} d={path(160, -90, -25, 1)} />
      <path className={gray} d="M0 49.9 H 100 V 50.1 H 0" />
    </svg>

    <div className={brand}>
      <div className={first}>Money</div>
      <div className={second}>Flow</div>
    </div>

    <form className={form}>
      <div>{process.env.API_BASE_URL}</div>

      <label className={label}>Email address</label>
      <input className={input} type="email" required />

      <label className={label}>Password</label>
      <input className={input} type="password" required />

      <button className={button}>Sign in</button>
    </form>
  </div>
);

render(<Root />, document.getElementById("root"));

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

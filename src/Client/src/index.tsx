import 'tslib';
import * as React from 'react';
import { render } from 'react-dom';
import css from "./index.less";

const Root = () => (
  <div className={css.account}>
  
    <svg className={css.background} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100" width="100%" height="100%" preserveAspectRatio="none">
      <path className={css.blue} d={path(100, 40, 0, 1)} />
      <path className={css.orange} d={path(160, 100, 50, 1)} />
      <path className={css.orange} d={path(170, -90, -25, 1)} />
      <path className={css.gray} d={path(160, -90, -25, 1)} />
      <path className={css.gray} d="M0 49.9 H 100 V 50.1 H 0" />
    </svg>

    <div className={css.brand}>
      <div className={css.first}>Money</div>
      <div className={css.second}>Flow</div>
    </div>

    <form className={css.form}>
      <label className={css.label}>Email address</label>
      <input className={css.input} type="email" required />

      <label className={css.label}>Password</label>
      <input className={css.input} type="password" required />

      <button className={css.button}>Sign in</button>
    </form>
  </div>
);

render(<Root />, document.getElementById('root'));

function path(period: number, amplitude: number, phase: number, repeat: number) {
  const line = new Array(repeat)
    .fill(0)
    .map((_, i) => i)
    .map(i => {
      const offset = phase + period*i;
      return `Q ${offset + period/4} ${50-amplitude}, ${offset + period/2} 50 T ${offset + period} 50`;
    })
    .join(" ");
  return `M${phase} 50 ${line}`;
}

import "tslib";
import * as React from "react";
import { render } from "react-dom";
import { account, brand, first, second, form, label, input, button } from './index.style';
import { Background } from "./background";

const Root = () => (
  <div className={account}>
    
    <Background />

    <div className={brand}>
      <div className={first}>Money</div>
      <div className={second}>Flow</div>
    </div>

    <form className={`pure-form ${form}`}>
      <div>{process.env.API_BASE_URL}</div>

      <label className={label}>Email address</label>
      <input className={input} type="email" required />

      <label className={label}>Password</label>
      <input className={input} type="password" required />

      <button className={`pure-button ${button}`}>Sign in</button>
    </form>
  </div>
);

render(<Root />, document.getElementById("root"));

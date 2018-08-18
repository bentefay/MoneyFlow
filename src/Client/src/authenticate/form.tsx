import * as React from "react";
import { css } from "emotion";
import { color6, color7, color4 } from '../palette.style';

export const c = {
  form: css({
    border: `1px solid ${color6}`,
    backgroundColor: color7,
    borderRadius: '5px',
    boxShadow: '5px 5px 20px #0004',
    padding: '40px',
    minWidth: '350px'
  }),
  label: css({
    fontWeight: 500
  }), 
  input: css({
    fontSize: '18px',
    width: '100%',
    marginTop: '10px !important',
    marginBottom: '30px !important'
  }),
  button: css({
    marginTop: '10px',
    padding: '12px 0',
    width: '100%',
    backgroundColor: color4,
    color: 'white',
  })
}

console.log(process.env.API_BASE_URL);

export const Form = () => (
    <form className={`pure-form pure-form-stacked ${c.form}`}>
      <label className={c.label}>Email address</label>
      <input className={c.input} type="email" required />

      <label className={c.label}>Password</label>
      <input className={c.input} type="password" required />

      <button className={`pure-button ${c.button}`}>Sign in or create account</button>
    </form>
);
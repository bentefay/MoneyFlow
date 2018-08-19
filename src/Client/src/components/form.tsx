import * as React from "react";
import { css } from "emotion";
import { color6, color7, color4 } from '../palette.style';
import { connect } from 'react-redux';
import { RootState } from '../store/store';
import { usernameUpdated, passwordUpdated, authSubmitted } from '../store/auth/actions';

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

interface Props {
  username: string;
  password: string;
}

interface Actions {
  updateUsername(value: string): void;
  updatePassword(value: string): void;
  submit(): void;
}

const form = ({ username, password, updateUsername, updatePassword, submit }: Props & Actions) => (
    <form className={`pure-form pure-form-stacked ${c.form}`}>
      <label className={c.label}>Email address</label>
      <input className={c.input} type="email" required value={username}
        onChange={event => updateUsername(event.currentTarget.value)} />

      <label className={c.label}>Password</label>
      <input className={c.input} type="password" required value={password}
        onChange={event => updatePassword(event.currentTarget.value)} />

      <button className={`pure-button ${c.button}`} onClick={event => { submit(); event.preventDefault(); }}>
        Sign in or create account
      </button>
    </form>
);

export const Form = connect(
  (state: RootState): Props => ({ 
    username: state.auth.username || "", 
    password: state.auth.password || "" 
  }), 
  (dispatch): Actions => ({
     updateUsername: value => dispatch(usernameUpdated({ username: value })),
     updatePassword: value => dispatch(passwordUpdated({ password: value })),
     submit: () => dispatch(authSubmitted())
 })
)(form);
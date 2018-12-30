import * as React from "react";
import { css } from "emotion";
import { color6, color7, color4, color5 } from '../styles/palette.style';
import { connect } from 'react-redux';
import { RootState } from '../../store/store';
import { Email, Password, AuthState, emailUpdated, passwordUpdated, loginInitiated, minimumPasswordLength, AuthStateValue } from '../../store/auth';
import { valueOrDefault } from '../../store/shared/functions';
import { Validation } from './validation';
import { GeneralFailureView } from './generalFailureView';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { ValidationErrors } from 'src/store/shared/models';
import { Loading } from './loading';

export const c = {
    form: css({
        position: "relative",
        border: `1px solid ${color6}`,
        backgroundColor: color7,
        borderRadius: '5px',
        boxShadow: '5px 5px 20px #0004',
        padding: '40px',
        width: '350px',
        "@media (max-width: 500px)": {
            margin: '0 20px',
            minWidth: '0',
            width: "auto"
        }
    }),
    label: css({
        fontWeight: 500
    }),
    inputDescription: css({
        fontSize: "80%",
        margin: "-20px 0 10px 0",
        color: "#888"
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
        color: 'white'
    })
};

type Props = AuthState

interface Actions {
    updateUsername(value: string, options: { revalidate: boolean }): void;
    updatePassword(value: string, options: { revalidate: boolean }): void;
    submit(): void;
}

const PasswordDescription = ({ password }: { password: Password | null }) => {
    if (password === null || password.value.length == 0)
        return <React.Fragment>Your password needs to be <b>12 or more</b> letters, numbers or symbols</React.Fragment>;

    const remainingCharactersRequired = minimumPasswordLength - password.value.length;

    if (remainingCharactersRequired > 1)
        return <React.Fragment>Just <b>{remainingCharactersRequired}</b> more letters, numbers or symbols</React.Fragment>;

    if (remainingCharactersRequired == 1)
        return <React.Fragment>Just <b>1</b> more letter, number or symbol!</React.Fragment>;

    return <React.Fragment></React.Fragment>;
}

const form = ({ value: { email, password }, errors, generalFailure, isLoading, updateUsername, updatePassword, submit }: Props & Actions) => (
    <form className={`pure-form pure-form-stacked ${c.form}`} noValidate>
        <Loading isLoading={isLoading} />

        <label className={c.label}>Email address</label>
        <Validation errors={errors.email}>
            <input className={c.input} type="text" value={valueOrDefault(email, "")} formNoValidate
                onChange={event => updateUsername(event.currentTarget.value, { revalidate: false })}
                onBlur={event => updateUsername(event.currentTarget.value, { revalidate: true })} />
        </Validation>

        <label className={c.label}>Password</label>
        <Validation errors={errors.password}>
            <input className={c.input} type="password" value={valueOrDefault(password, "")} formNoValidate
                onChange={event => updatePassword(event.currentTarget.value, { revalidate: false })}
                onBlur={event => updatePassword(event.currentTarget.value, { revalidate: true })} />
        </Validation>
        <div className={c.inputDescription}>
            <PasswordDescription password={password} />
        </div>

        <GeneralFailureView value={generalFailure} />

        <button className={`pure-button ${c.button}`}
            style={{ backgroundColor: isAuthStateValid(errors) ? color4 : color5 }}
            onClick={event => { submit(); event.preventDefault(); }}>
            <FontAwesomeIcon fixedWidth icon="unlock" /> Sign in or create account
        </button>
    </form>
);

export function isAuthStateValid(errors: ValidationErrors<AuthStateValue>) {
    return (errors.email == undefined || errors.email.length == 0) &&
        (errors.password == undefined || errors.password.length == 0)
}

export const Form = connect(
    (state: RootState): Props => state.auth,
    (dispatch): Actions => ({
        updateUsername: (value, options) => dispatch(emailUpdated({ email: new Email(value), revalidate: options.revalidate })),
        updatePassword: (value, options) => dispatch(passwordUpdated({ password: new Password(value), revalidate: options.revalidate })),
        submit: () => dispatch(loginInitiated())
    })
)(form);
import * as React from "react";
import { css } from "emotion";
import { color6, color7, color4 } from '../styles/palette.style';
import { connect } from 'react-redux';
import { RootState } from '../../store/store';
import { Email, Password, AuthState, emailUpdated, passwordUpdated, loginInitiated, minimumPasswordLength, AuthStateValue, AuthView, toggleAuthView } from '../../store/auth';
import { valueOrDefault } from '../../store/shared/functions';
import { Validation } from './validation';
import { GeneralFailureView } from './generalFailureView';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { ValidationErrors } from 'src/store/shared/models';
import { Loading } from './loading';

export const s = {
    button: css({
        marginTop: '20px',
        padding: '12px 0',
        width: '100%',
        cursor: "pointer"
    }),
}

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
        },
        transition: "height 2s"
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
    buttonRow: css({
        display: "flex"
    }),
    signIn: css({
        color: 'white',
        backgroundColor: color4
    }, s.button),
    signUp: css({
        fontSize: "90%",
        backgroundColor: color7,
        textDecorationLine: "underline",
        color: "#888"
    }, s.button),
    createAccountHeading: css({
        marginBottom: "20px",
        fontSize: "120%"
    }),
    createAccountDescription: css({
        fontSize: "80%",
        margin: "30px 0",
        color: "#888"
    }),
};

type Props = AuthState

interface Actions {
    updateUsername(value: string, options: { revalidate: boolean }): void;
    updatePassword(value: string, options: { revalidate: boolean }): void;
    submit(): void;
    toggleAuthView(): void;
}

const PasswordDescription = ({ password, loggingIn }: { password: Password | null, loggingIn: boolean }) => {
    if (password === null || password.value.length == 0)
        return <>Must be <b>12 or more</b> letters, numbers or symbols</>;

    const remainingCharactersRequired = minimumPasswordLength - password.value.length;

    if (remainingCharactersRequired > 1)
        return <>Just <b>{remainingCharactersRequired}</b> more letters, numbers or symbols</>;

    if (remainingCharactersRequired == 1)
        return <>Just <b>1</b> more letter, number or symbol!</>;

    return loggingIn ? null : <>Great password!</>;
}

const form = ({ value: { email, password }, errors, generalFailure, isLoading, toggleAuthView, updateUsername, updatePassword, submit, view }: Props & Actions) => (
    <form className={`pure-form pure-form-stacked ${c.form}`} noValidate>
        <Loading isLoading={isLoading} />

        {
            view == AuthView.createAccount ?
                <>
                    <div className={c.createAccountHeading}>Let's create an <span style={{ color: "#00abe3" }}>account!</span></div>
                </> :
                null
        }

        <label className={c.label}>Email</label>
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
        {
            (password != null && password.value != "") || view == AuthView.createAccount ?
                <div className={c.inputDescription}>
                    <PasswordDescription password={password} loggingIn={view == AuthView.logIn} />
                </div> :
                null
        }

        <GeneralFailureView value={generalFailure} />

        <div className={c.buttonRow}>
            <a className={`${c.signUp}`}
                onClick={event => { toggleAuthView(); event.preventDefault(); }}>
                {
                    view == AuthView.logIn ?
                        <>Create an account</> :
                        <>Log in to an account</>
                }

            </a>

            <button className={`pure-button ${c.signIn}`} disabled={!isAuthStateValid(errors)}
                onClick={event => { submit(); event.preventDefault(); }}>
                {
                    view == AuthView.logIn ?
                        <><FontAwesomeIcon fixedWidth icon="unlock" /> Sign in</> :
                        <><FontAwesomeIcon fixedWidth icon="plus" /> Create</>
                }
            </button>
        </div>

    </form>
);

function isAuthStateValid(errors: ValidationErrors<AuthStateValue>) {
    return (errors.email == undefined || errors.email.length == 0) &&
        (errors.password == undefined || errors.password.length == 0)
}

export const Form = connect(
    (state: RootState): Props => state.auth,
    (dispatch): Actions => ({
        updateUsername: (value, options) => dispatch(emailUpdated({ email: new Email(value), revalidate: options.revalidate })),
        updatePassword: (value, options) => dispatch(passwordUpdated({ password: new Password(value), revalidate: options.revalidate })),
        submit: () => dispatch(loginInitiated()),
        toggleAuthView: () => dispatch(toggleAuthView())
    })
)(form);
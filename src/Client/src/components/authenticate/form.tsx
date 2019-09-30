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
import useMeasure from "use-measure";
import { useSpring, animated } from 'react-spring'

export const common = {
    button: css({
        marginTop: '20px',
        padding: '12px 0',
        width: '100%',
        cursor: "pointer"
    }),
}

export const c = {
    formContainer: css({
    }),
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
        fontWeight: 400
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
    submitButton: css({
        color: 'white',
        backgroundColor: color4,
        fontSize: "80%",
        letterSpacing: "1.2",
        textTransform: "uppercase"
    }, common.button),
    submitButtonIcon: css({
        marginRight: "0.5em",
        marginLeft: "-0.5em"
    }),
    alternateActionButton: css({
        fontSize: "90%",
        backgroundColor: color7,
        textDecorationLine: "underline",
        color: "#888"
    }, common.button),
    createAccountHeading: css({
        marginBottom: "20px",
        fontSize: "110%",
        fontWeight: 500
    }),
    createAccountDescription: css({
        fontSize: "80%",
        marginBottom: "30px",
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

const form = ({ value: { email, password }, errors, generalFailure, isLoading, toggleAuthView, updateUsername, updatePassword, submit, view }: Props & Actions) => {

    return (
        <form className={`pure-form pure-form-stacked ${c.form}`} noValidate>
            <Loading isLoading={isLoading} />

            {
                view == AuthView.createAccount ?
                    <>
                        <div className={c.createAccountHeading}>Create Account</div>
                        <div className={c.createAccountDescription}>We do not ever see your password, so we cannot reset it for you.</div>
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
                <a className={`${c.alternateActionButton}`}
                    onClick={event => { toggleAuthView(); event.preventDefault(); }}>
                    {
                        view == AuthView.logIn ?
                            <>Create account</> :
                            <>Log in to account</>
                    }

                </a>

                <button className={`pure-button ${c.submitButton}`} disabled={!isAuthStateValid(errors)}
                    onClick={event => { submit(); event.preventDefault(); }}>
                    {
                        view == AuthView.logIn ?
                            <><FontAwesomeIcon fixedWidth icon="unlock" size="sm" className={c.submitButtonIcon} /> Sign in</> :
                            <><FontAwesomeIcon fixedWidth icon="user-plus" size="sm" className={c.submitButtonIcon} /> Create</>
                    }
                </button>
            </div>

        </form>
    )
};

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
import * as React from "react";
import { css } from "emotion";
import { color6, color7, color4 } from '../styles/palette.style';
import { connect } from 'react-redux';
import _ from 'lodash';
import { RootState } from '../../store/store';
import { AuthState, loginInitiated, minimumPasswordLength, UserCredentials, createAccountToggled, Email, Password, validateEmail, validatePassword } from '../../store/auth';
import { renderFormField } from './validation';
import { GeneralFailureView } from './generalFailureView';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { formField, useFormState, FormStateValidator, FormState } from '../../store/shared/models';
import { Loading } from './loading';

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
    onLoginInitiated(options: { credentials: UserCredentials, create: boolean }): void;
    onCreateAccountToggled(options: { createAccount: boolean }): void;
}

const PasswordDescription = ({ password, loggingIn }: { password: string | null, loggingIn: boolean }) => {
    if (password === null || password.length == 0)
        return <>Must be <b>12 or more</b> letters, numbers or symbols</>;

    const remainingCharactersRequired = minimumPasswordLength - password.length;

    if (remainingCharactersRequired > 1)
        return <>Just <b>{remainingCharactersRequired}</b> more letters, numbers or symbols</>;

    if (remainingCharactersRequired == 1)
        return <>Just <b>1</b> more letter, number or symbol!</>;

    return loggingIn ? null : <>Great password!</>;
}

type AuthForm = { email: string | null, password: string | null };

const defaultForm : FormState<AuthForm> = {
    email: formField(null), 
    password: formField(null) 
};

const formValidator : FormStateValidator<AuthForm> = state => {
    return {
        email: state.email.touched ? validateEmail(state.email.value) : undefined,
        password: state.password.touched ? validatePassword(state.password.value) : undefined
    };
};

const form = ({ createAccount, generalFailure, isLoading, onCreateAccountToggled, onLoginInitiated }: Props & Actions) => {
    const  { state, onChange, onSubmit, isValid, reset } = useFormState<AuthForm>(
        () => defaultForm, 
        formValidator,
        state => onLoginInitiated({ 
            credentials: { 
                email: new Email(state.email!), 
                password: new Password(state.password!) 
            }, 
            create: createAccount 
        }));

    const { password } = state;

    return (
        <form className={`pure-form pure-form-stacked ${c.form}`} noValidate>
            <Loading isLoading={isLoading} />

            {
                createAccount ?
                    <>
                        <div className={c.createAccountHeading}>Create Account</div>
                        <div className={c.createAccountDescription}>Your account is private. We never see your password, or your data.</div>
                    </> :
                    null
            }

            <label className={c.label}>Email</label>
            {renderFormField(state, "email", onChange, ({ onChange, onBlur, value }) => 
                <input className={c.input} type="text" value={value || ""} formNoValidate onChange={onChange} onBlur={onBlur} />
            )}

            <label className={c.label}>Password</label>
            {renderFormField(state, "password", onChange, ({ onChange, onBlur, value }) => 
                <input className={c.input} type="password" value={value || ""} formNoValidate onChange={onChange} onBlur={onBlur} />
            )}
            {
                (password.value != null && password.value != "") || createAccount ?
                    <div className={c.inputDescription}>
                        <PasswordDescription password={password.value} loggingIn={!createAccount} />
                    </div> :
                    null
            }

            <GeneralFailureView value={generalFailure} />

            <div className={c.buttonRow}>
                <a className={`${c.alternateActionButton}`}
                    onClick={event => { reset(); onCreateAccountToggled({ createAccount: !createAccount }); event.preventDefault(); }}>
                    {
                        createAccount ?
                            <>Log in to account</> :
                            <>Create account</>
                    }

                </a>

                <button className={`pure-button ${c.submitButton}`} disabled={!isValid} onClick={event => { onSubmit(); event.preventDefault(); }}>
                    {
                        createAccount ?
                            <><FontAwesomeIcon fixedWidth icon="user-plus" size="sm" className={c.submitButtonIcon} /> Create</> :
                            <><FontAwesomeIcon fixedWidth icon="unlock" size="sm" className={c.submitButtonIcon} /> Sign in</>
                    }
                </button>
            </div>

        </form>
    )
};

export const Form = connect(
    (state: RootState): Props => state.auth,
    (dispatch): Actions => ({
        onLoginInitiated: options => dispatch(loginInitiated(options)),
        onCreateAccountToggled: options => dispatch(createAccountToggled(options))
    })
)(form);
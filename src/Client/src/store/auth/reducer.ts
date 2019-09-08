import { getType } from "typesafe-actions";
import { AuthAction, AuthState, authActions } from '.';
import { validatePassword, validateEmail, validate } from './validation';
import { Email, Password, AuthView } from './model';

const getDefaultState = (): AuthState => {
    return {
        view: AuthView.logIn,
        value: {
            email: null,
            password: null
        },
        isLoading: false,
        errors: {}
    };
}

export const authReducer = (state = getDefaultState(), action: AuthAction): AuthState => {
    switch (action.type) {
        case getType(authActions.toggleAuthView):
            return {
                ...state,
                view: state.view == AuthView.logIn ? AuthView.createAccount : AuthView.logIn,
                errors: {}
            };

        case getType(authActions.emailUpdated):
            return {
                ...state,
                value: { ...state.value, email: action.payload.email },
                errors: { ...state.errors, email: validate(action.payload.email, validateEmail, state.errors.email, action.payload.revalidate) }
            };

        case getType(authActions.passwordUpdated):
            return {
                ...state,
                value: { ...state.value, password: action.payload.password },
                errors: { ...state.errors, password: validate(action.payload.password, validatePassword, state.errors.password, action.payload.revalidate) }
            };

        case getType(authActions.loginErrored):
            return action.payload.type == "generalFailure" ?
                { ...state, generalFailure: action.payload, isLoading: false } :
                { ...state, errors: action.payload.errors, isLoading: false };

        case getType(authActions.loginInitiated):
            const errors = {
                email: validateEmail(state.value.email || new Email("")),
                password: validatePassword(state.value.password || new Password("")),
            };
            return {
                ...state,
                generalFailure: undefined,
                isLoading: errors.email.length == 0 && errors.password.length == 0,
                errors: errors
            };

        default:
            return state;
    }
}
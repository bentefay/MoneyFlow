import { getType } from "typesafe-actions";
import { AuthAction, AuthState, authActions } from '.';
import { withValue } from '../shared/functions';
import { validatePassword, validateEmail, validate } from './validation';
import { Email, Password } from './model';

const getDefaultState = (): AuthState => {
    return {
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
        case getType(authActions.emailUpdated):
            return withValue(state, {
                value: { email: action.payload.email },
                errors: { email: validate(action.payload.email, validateEmail, state.errors.email, action.payload.revalidate) }
            });

        case getType(authActions.passwordUpdated):
            return withValue(state, {
                value: { password: action.payload.password },
                errors: { password: validate(action.payload.password, validatePassword, state.errors.password, action.payload.revalidate) }
            });

        case getType(authActions.loginErrored):
            return action.payload.type == "generalFailure" ?
                withValue(state, { generalFailure: action.payload, isLoading: false }) :
                withValue(state, { errors: action.payload.errors, isLoading: false });

        case getType(authActions.loginInitiated):
            return withValue(state, {
                generalFailure: undefined,
                isLoading: true,
                errors: {
                    email: validateEmail(state.value.email || new Email("")),
                    password: validatePassword(state.value.password || new Password("")),
                }
            });

        default:
            return state;
    }
}
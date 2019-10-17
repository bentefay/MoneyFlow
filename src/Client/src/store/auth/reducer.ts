import { getType } from "typesafe-actions";
import { AuthAction, AuthState, authActions } from '.';

const getDefaultState = (): AuthState => {
    return {
        createAccount: false,
        isLoading: false
    };
}

export const authReducer = (state = getDefaultState(), action: AuthAction): AuthState => {
    switch (action.type) {
        case getType(authActions.createAccountToggled):
            return {
                ...state,
                generalFailure: undefined,
                createAccount: action.payload.createAccount
            };

        case getType(authActions.loginInitiated):
            return {
                ...state,
                credentials: action.payload.credentials,
                isLoading: true
            };

        case getType(authActions.loginErrored):
            return { ...state, generalFailure: action.payload, isLoading: false };

        default:
            return state;
    }
}
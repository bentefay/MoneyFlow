import { getType } from "typesafe-actions";
import { AuthAction, AuthState, authActions } from ".";

const getDefaultState = (): AuthState => {
    return {
        createAccount: false,
        isLoading: false,
    };
};

export const authReducer = (state = getDefaultState(), action: AuthAction): AuthState => {
    switch (action.type) {
        case getType(authActions.createAccountToggled):
            return {
                ...state,
                error: undefined,
                createAccount: action.payload.createAccount,
            };

        case getType(authActions.signInInitiated):
            return {
                ...state,
                error: undefined,
                credentials: action.payload.credentials,
                isLoading: true,
            };

        case getType(authActions.signInErrored):
            return { ...state, error: action.payload, isLoading: false };

        default:
            return state;
    }
};

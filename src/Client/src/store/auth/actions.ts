import { createAction } from "typesafe-actions";
import { UserCredentials } from ".";
import { EncryptedVault, NewVaultPlaceholder, AuthStateError } from "./model";
import { ActionType } from "typesafe-actions";

export const authActions = {
    createAccountToggled: createAction("CREATE_ACCOUNT_TOGGLED")<{ createAccount: boolean }>(),
    signInInitiated: createAction("SIGN_IN_INITIATED")<{ credentials: UserCredentials; create: boolean }>(),
    signInSucceeded: createAction("SIGN_IN_SUCCEEDED")<NewVaultPlaceholder | EncryptedVault>(),
    signInErrored: createAction("SIGN_IN_ERRORED")<AuthStateError>(),
    requestAborted: createAction("REQUEST_ABORTED")()
};

export type AuthAction = ActionType<typeof authActions>;
export type SignInInitiated = ReturnType<typeof authActions.signInInitiated>;

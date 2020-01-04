import { createAction } from "typesafe-actions";
import { GeneralFailure } from '../shared/models';
import { UserCredentials } from '.';
import { EncryptedVault, NewVaultPlaceholder } from './model';
import { ActionType } from "typesafe-actions";

export const authActions = {
    createAccountToggled: createAction("CREATE_ACCOUNT_TOGGLED")<{ createAccount: boolean }>(),
    loginInitiated: createAction("LOGIN_INITIATED")<{ credentials: UserCredentials, create: boolean }>(),
    loginSucceeded: createAction("LOGIN_SUCCEEDED")<NewVaultPlaceholder | EncryptedVault>(),
    loginErrored: createAction("LOGIN_ERRORED")<GeneralFailure>(),
    requestAborted: createAction("REQUEST_ABORTED")()
};

export type AuthAction = ActionType<typeof authActions>;
export type LoginInitiated = ReturnType<typeof authActions.loginInitiated>;
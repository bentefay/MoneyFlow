import { createAction } from "typesafe-actions";
import { GeneralFailure } from '../shared/models';
import { UserCredentials } from '.';
import { EncryptedVault, NewVaultPlaceholder } from './model';

export const createAccountToggled = createAction("CREATE_ACCOUNT_TOGGLED")<{ createAccount: boolean }>();
export const loginInitiated = createAction("LOGIN_INITIATED")<{ credentials: UserCredentials, create: boolean }>();
export const loginSucceeded = createAction("LOGIN_SUCCEEDED")<NewVaultPlaceholder | EncryptedVault>();
export const loginErrored = createAction("LOGIN_ERRORED")<GeneralFailure>();
export const requestAborted = createAction("REQUEST_ABORTED")();

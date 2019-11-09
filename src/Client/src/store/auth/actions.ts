import { createAction } from "typesafe-actions";
import { GeneralFailure } from '../shared/models';
import { CreateVaultResponse, UserCredentials } from '.';

export const createAccountToggled = createAction("CREATE_ACCOUNT_TOGGLED")<{ createAccount: boolean }>();
export const loginInitiated = createAction("LOGIN_INITIATED")<{ credentials: UserCredentials, create: boolean }>();
export const loginSucceeded = createAction("LOGIN_SUCCEEDED")<CreateVaultResponse>();
export const loginErrored = createAction("LOGIN_ERRORED")<GeneralFailure>();

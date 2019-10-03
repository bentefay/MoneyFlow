import { createStandardAction } from "typesafe-actions";
import { GeneralFailure } from '../shared/models';
import { EncryptedVault, UserCredentials } from '.';

export const createAccountToggled = createStandardAction("CREATE_ACCOUNT_TOGGLED")<{ createAccount: boolean }>();
export const loginInitiated = createStandardAction("LOGIN_INITIATED")<{ credentials: UserCredentials, create: boolean }>();
export const loginSucceeded = createStandardAction("LOGIN_SUCCEEDED")<EncryptedVault>();
export const loginErrored = createStandardAction("LOGIN_ERRORED")<GeneralFailure>();

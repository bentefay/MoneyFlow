import { createStandardAction } from "typesafe-actions";
import { GeneralFailure, Invalid } from '../shared/models';
import { Email, Password, EncryptedVault, AuthStateValue } from '.';

export const emailUpdated = createStandardAction("USERNAME_UPDATED")<{ email: Email, revalidate: boolean }>();
export const passwordUpdated = createStandardAction("PASSWORD_UPDATED")<{ password: Password, revalidate: boolean }>();
export const loginInitiated = createStandardAction("LOGIN_INITIATED")();
export const loginSucceeded = createStandardAction("LOGIN_SUCCEEDED")<EncryptedVault>();
export const loginErrored = createStandardAction("LOGIN_ERRORED")<GeneralFailure | Invalid<AuthStateValue>>();
export const userAccountNotFound = createStandardAction("CREATE_ACCOUNT_REQUESTED")<{ email: Email, password: Password }>();

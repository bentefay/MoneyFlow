import { createStandardAction } from "typesafe-actions";
import { GeneralFailure, Invalid } from '../shared/model';
import { AuthState, Username, Password, EncryptedVault } from '.';

export const usernameUpdated = createStandardAction("USERNAME_UPDATED")<{ username: Username }>();
export const passwordUpdated = createStandardAction("PASSWORD_UPDATED")<{ password: Password }>();
export const loginInitiated = createStandardAction("LOGIN_INITIATED")<{ username: Username, password: Password }>();
export const loginSucceeded = createStandardAction("LOGIN_SUCCEEDED")<EncryptedVault>();
export const loginErrored = createStandardAction("LOGIN_ERRORED")<GeneralFailure | Invalid<AuthState>>();
export const userAccountNotFound = createStandardAction("CREATE_ACCOUNT_REQUESTED")<{ username: Username, password: Password }>();

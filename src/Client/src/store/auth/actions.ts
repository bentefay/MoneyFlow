import { createStandardAction } from "typesafe-actions";
import { Failure } from '.';

export const usernameUpdated = createStandardAction("USERNAME_UPDATED")<{ username: string }>();
export const passwordUpdated = createStandardAction("PASSWORD_UPDATED")<{ password: string }>();
export const loginOrCreateInitiated = createStandardAction("LOGIN_OR_CREATE_INITIATED")();
export const loginOrCreateCompleted = createStandardAction("LOGIN_OR_CREATE_COMPLETED")();
export const loginOrCreateErrored = createStandardAction("LOGIN_OR_CREATE_ERRORED")<Failure>();

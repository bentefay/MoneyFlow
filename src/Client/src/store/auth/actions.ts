import { createStandardAction } from "typesafe-actions";

const USERNAME_UPDATED = "USERNAME_UPDATED";
const PASSWORD_UPDATED = "PASSWORD_UPDATED";
const AUTH_SUBMITTED = "AUTH_SUBMITTED";

export const usernameUpdated = createStandardAction(USERNAME_UPDATED)<{ username: string }>();
export const passwordUpdated = createStandardAction(PASSWORD_UPDATED)<{ password: string }>();
export const authSubmitted = createStandardAction(AUTH_SUBMITTED)();

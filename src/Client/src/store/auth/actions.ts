import { createStandardAction } from "typesafe-actions";

export const usernameUpdated = createStandardAction("USERNAME_UPDATED")<{ username: string }>();
export const passwordUpdated = createStandardAction("PASSWORD_UPDATED")<{ password: string }>();
export const loginOrCreateInitiated = createStandardAction("LOGIN_OR_CREATE_INITIATED")();
export const loginOrCreateCompleted = createStandardAction("LOGIN_OR_CREATE_COMPLETED")();

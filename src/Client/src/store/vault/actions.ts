import { createStandardAction } from "typesafe-actions";

const VAULT_REQUESTED = "GET_VAULT";
const VAULT_REQUEST_COMPLETED = "VAULT_REQUEST_COMPLETED";

export const vaultRequested = createStandardAction(VAULT_REQUESTED)<{ email: string, secret: string }>();
export const vaultRequestCompleted = createStandardAction(VAULT_REQUEST_COMPLETED)<{ id: string }>();

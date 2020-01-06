import { createAction } from "typesafe-actions";
import { ActionType } from "typesafe-actions";
import { NewVaultPlaceholder, EncryptedVault } from "../shared/models";

export const vaultActions = {
    vaultReceived: createAction("VAULT_RECEIVED")<{ vault: NewVaultPlaceholder | EncryptedVault }>()
};

export type VaultActions = ActionType<typeof vaultActions>;
export type VaultReceived = ReturnType<typeof vaultActions.vaultReceived>;
